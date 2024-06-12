using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json.Serialization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace MonoGo.Engine.Particles
{
    public class ParticleEffect
    {
        public string Name { get; set; } = "Particle Effect";
        public Emitter[] Emitters { get; set; }

        [JsonIgnore]
        public bool StopEmitting { get; set; } = false;

        public T[] Modifiers<T>(string name = default)
        {
            return Emitters
                .SelectMany(e => e.Modifiers)
                .Where(m => m is T && (string.IsNullOrEmpty(name) || m.Key == name))
                .Select(m => (T)m).ToArray();
        }

        public void SetLoop(string emitterName)
        {
            Emitters.Where(x => x.Name == emitterName).ToList().ForEach(x => x.Loop = !x.Loop);
        }

        public void SetDraw(string emitterName)
        {
            Emitters.Where(x => x.Name == emitterName).ToList().ForEach(x => x.Draw_b = !x.Draw_b);
        }

        public void ReorderEmitterArray()
        {
            Emitter[] ordered = Emitters.OrderBy(x => x.LayerDepth).ToArray();
            Emitters = ordered;
        }

        public void StartEmitters()
        {
            Emitters.ToList().ForEach(
                x =>
                {
                    x._CurrentTime = x._MaxTime;
                    x.StopEmitting = false;
                });
        }

        public void StopEmitters()
        {
            Emitters.ToList().ForEach(x => x.StopEmitting = true);
            StopEmitting = true;
        }

        public ParticleEffect()
        {
            Emitters = new Emitter[0];
        }

        /// <summary>
        /// Deserialization ctor.
        /// </summary>
        /// <param name="filePath">The absolut file path of the particle effect file in JSON format.</param>
        public ParticleEffect(string filePath)
        {
            var particleEffect = Deserialize(filePath);
            
            Name = particleEffect.Name;
            StopEmitting = particleEffect.StopEmitting;
            Emitters = particleEffect.Emitters;
        }

        public ParticleEffect Clone()
        {
            var particleEffect = new ParticleEffect
            {
                Name = Name,
                StopEmitting = StopEmitting,
                Emitters = Emitters.Select(x => x.Clone()).ToArray()
            };

            return particleEffect;
        }

        public FileInfo Serialize(string filePath)
        {
            var pathInfo = new FileInfo(@Path.Combine(filePath, Name));
            if (!Directory.Exists(filePath)) Directory.CreateDirectory(filePath);

            var json = JsonSerializer.Serialize(this, JsonConverters.SerializerOptions);
            File.WriteAllText($"{pathInfo}.mpe", json, Encoding.UTF8);

            return pathInfo;
        }

        private ParticleEffect Deserialize(string filePath)
        {
            return JsonSerializer.Deserialize<ParticleEffect>(
                    File.ReadAllText($"{filePath}.mpe"), JsonConverters.SerializerOptions);
        }

        [JsonIgnore]
        public int ActiveParticles => Emitters.Sum(t => t.ActiveParticles);

        public void FastForward(Vector2 position, float seconds, float triggerPeriod)
        {
            var time = 0f;
            while (time < seconds)
            {
                Update(triggerPeriod);
                Trigger(position);
                time += triggerPeriod;
            }
        }

        public void Update(float elapsedSeconds)
        {
            foreach (var e in Emitters)
                e.Update(elapsedSeconds);
        }

        public void Draw()
        {
            foreach (Emitter emitter in Emitters)
                emitter.Draw();
        }

        public void Draw(BasicEffect effect)
        {
            foreach (Emitter emitter in Emitters)
                emitter.Draw(effect);
        }

        public void Trigger(Vector2 position)
        {
            foreach (var e in Emitters)
                e.Trigger(new Vector2(position.X, position.Y));
        }

        public void Trigger(LineSegment line)
        {
            foreach (var e in Emitters)
                e.Trigger(line);
        }
    }
}