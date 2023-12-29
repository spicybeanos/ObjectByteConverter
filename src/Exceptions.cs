

namespace ByteConverter
{
    public class NotUserDefinedException : Exception
    {
        public NotUserDefinedException(Type type) : base($"'{type}' is not user defined type")
        {
        }

        public NotUserDefinedException(DataTypeID type) : base($"'{type}' is not user defined type")
        {
        }
    }

    public class CouldNotDefineTypeException : Exception
    {
        public CouldNotDefineTypeException(Type type,Exception ex) : base($"Could not add type '{type}' to class definitions: \n{ex}")
        {
        }
    }
}