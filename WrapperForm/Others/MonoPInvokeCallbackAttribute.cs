namespace TestForm
{
    [AttributeUsage(AttributeTargets.Method)]
    public class PInvokeCallbackAttribute : Attribute
    {
        public PInvokeCallbackAttribute(Type type)
        {
        }
    }
}
