namespace HelloWorld
{
    /// <summary>
    /// Hello this is **Class1** from *HelloDocfx*
    /// </summary>
    public class HClass1
    {
        private InnerClass _Hclass;
        public int HValue { get; }

        /// <summary>
        /// This is a ctor
        /// </summary>
        /// <param name="value">The value of the class</param>
        public HClass1(int value)
        {
            HValue = value;
        }

        public double HConvertToDouble()
        {
            return HValue;
        }

        /// <summary>
        /// A method referencing a inner class
        /// </summary>
        /// <param name="name">The name</param>
        /// <param name="inner">A inner class with type <seealso cref="InnerClass"/></param>
        public void HSetInnerClass(string name, InnerClass inner)
        {
            inner.Name = name;
            _class = inner;
        }

        public class InnerClass
        {
            public string Name { get; set; }
        }
    }
}