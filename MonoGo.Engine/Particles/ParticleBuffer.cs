﻿using System;
using System.Runtime.InteropServices;

namespace MonoGo.Engine.Particles
{
    public class ParticleBuffer : IDisposable {
        private readonly IntPtr _nativePointer;
        public readonly int Size;

        public unsafe ParticleBuffer(int size) {
            Size = size;
            // add one extra spot in memory for margin between head and tail
            // so the iterator can see that it's at the end
            _nativePointer = Marshal.AllocHGlobal(SizeInBytes);
            BufferEnd = (Particle*) (_nativePointer + SizeInBytes);
            Head = (Particle*) _nativePointer;
            Tail = (Particle*) _nativePointer;

            _iterator = new ParticleIterator(this);

            GC.AddMemoryPressure(SizeInBytes);
        }

        private readonly ParticleIterator _iterator;

        public ParticleIterator Iterator => _iterator.Reset();

        // points to the first memory pos after the buffer
        protected readonly unsafe Particle* BufferEnd;
        // pointer to the first particle
        public unsafe Particle* Head { get; private set; }
        // points to the particle after the last active particle.
        protected unsafe Particle* Tail;

        // Number of available particle spots in the buffer
        public int Available => Size - Count;
        // current number of particles
        public int Count { get; private set; }
        // total size of the buffer
        public int SizeInBytes => Particle.SizeInBytes * (Size + 1);
        // total size of active particles
        public int ActiveSizeInBytes => Particle.SizeInBytes * Count;

        /// <summary>
        /// Release the given number of particles or the most available.
        /// Returns a started iterator to iterate over the new particles.
        /// </summary>
        public unsafe ParticleIterator Release(int releaseQuantity)
        {
            var numToRelease = Math.Min(releaseQuantity, Available);

            var prevCount = Count;
            Count += numToRelease;

            Tail += numToRelease;
            if (Tail >= BufferEnd) Tail -= Size + 1;

            return Iterator.Reset(prevCount);
        }

        public unsafe void Reclaim(int number) {
            Count -= number;

            Head += number;
            if (Head >= BufferEnd)
                Head -= (Size + 1);
        }

        private bool _disposed;

        public void Dispose() 
        {
            if (!_disposed) 
            {
                Marshal.FreeHGlobal(_nativePointer);
                _disposed = true;

                if (Size > 0) GC.RemoveMemoryPressure(Particle.SizeInBytes * Size);
            }            
            GC.SuppressFinalize(this);
        }

        ~ParticleBuffer() {
            Dispose();
        }

        public class ParticleIterator
        {
            private readonly ParticleBuffer _buffer;

            private unsafe Particle* _current;
            public unsafe bool HasNext => _current != _buffer.Tail;

            public int Total;

            public ParticleIterator(ParticleBuffer buffer)
            {
                _buffer = buffer;
            }
            
            public unsafe ParticleIterator Reset()
            {
                _current = _buffer.Head;
                Total = _buffer.Count;
                return this;
            }

            internal unsafe ParticleIterator Reset(int offset)
            {
                Total = _buffer.Count;

                _current = _buffer.Head + offset;
                if (_current >= _buffer.BufferEnd)
                    _current -= _buffer.Size + 1;

                return this;
            }
            
            public unsafe Particle* Next()
            {
                var p = _current;
                _current++;
                if (_current == _buffer.BufferEnd)
                    _current = (Particle*)_buffer._nativePointer;

                return p;
            }
        }

    }
}
