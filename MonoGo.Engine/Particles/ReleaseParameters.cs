namespace MonoGo.Engine.Particles
{
    public class ReleaseParameters
    {
        //Json ctr
        protected ReleaseParameters(bool json) { }

        public ReleaseParameters() 
        {
            Quantity = 2;
            Speed    = RangeF.Parse("[10.0,30.0]");
            Colour   = new ColourRange(new Colour(0f, 0.5f, 0.5f), new Colour(359f, 0.5f, 0.5f));
            Opacity  = RangeF.Parse("[1.0,1.0]");
            Scale    = RangeF.Parse("[5.0,10.0]");
            Rotation = RangeF.Parse("[-3.14159,3.14159]");
            Mass     = 1f;
        }

        public Range Quantity { get; set; }
        public RangeF Speed { get; set; }
        public ColourRange Colour { get; set; }
        public RangeF Opacity { get; set; }
        public RangeF Scale { get; set; }
        public RangeF Rotation { get; set; }
        public RangeF Mass { get; set; }

        public override string ToString()
        {
            return GetType().ToString();
        }
    }
}