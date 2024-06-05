using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MonoGo.Engine.Particles.Modifiers
{
    public class FollowObjectModifier : IModifier
    {
        public class FollowObjectModifierConverter : JsonConverter<IModifier>
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(FollowObjectModifier));
            }

            public override IModifier Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                return new FollowObjectModifier();
            }

            public override void Write(Utf8JsonWriter writer, IModifier value, JsonSerializerOptions options)
            {
            }
        }

        public dynamic ObjectReference { get; set; }
        public Vector Offset { get; set; }

        public unsafe void Update(float elapsedSeconds, ParticleBuffer.ParticleIterator iterator)
        {
            while (iterator.HasNext)
            {
                var particle = iterator.Next();
                Vector position = particle->Position;
                if (ObjectReference != null)
                {
                    position = new Vector(ObjectReference.Position.X + Offset.X, -ObjectReference.Position.Y + Offset.Y);
                }
                particle->Position = position;
            }
        }

        public override string ToString()
        {
            return GetType().ToString();
        }

        public void UpdateReferences(ref object _object)
        {
            if (((dynamic)_object).GetListViewItemTagDataReference.GetItemType.ToString() == "Object")
            {
                string id = ((dynamic)_object).GetListViewItemTagDataReference.ID.ToString();
                var bodyList = ((dynamic)_object).WorldReference.BodyList;
                foreach (dynamic body in bodyList)
                {
                    if (body.ID.ToString() == id)
                    {
                        ObjectReference = body;
                        break;
                    }
                }
            }
            else
            {
                string objectType = _object.GetType().ToString();
                if (objectType.Contains("Entity")) ObjectReference = ((dynamic)_object).PivotBody;
            }
        }
    }
}
