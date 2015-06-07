using System;

namespace Y2LCore.SyntaxTree
{
    public enum PrimitiveType
    {
        LuanLy,
        Nguyen,
        Thuc,
        Chuoi,
        Khong,
        Unknown,
        Null,
    }
    [Serializable]
    public class TypedObject
    {
        public PrimitiveType PrimitiveType;
        public object Value;

        public TypedObject()
        {
        }
        public TypedObject(PrimitiveType pType, object value)
        {
            this.PrimitiveType = pType;
            this.Value = value;
        }
        public override string ToString()
        {
            return this.PrimitiveType.ToString() + " " + Value ?? Value.ToString();
        }
        public static TypedObject Parse(object value)
        {
            if (value == null)
                return new TypedObject(PrimitiveType.Null, null);
            Type type = value.GetType();

            if (type == typeof(bool))
                return new TypedObject(PrimitiveType.LuanLy, value);
            if (type == typeof(int))
                return new TypedObject(PrimitiveType.Nguyen, value);
            if (type == typeof(double))
                return new TypedObject(PrimitiveType.Thuc, value);
            if (type == typeof(string))
                return new TypedObject(PrimitiveType.Chuoi, value);

            throw new Exception("The type '" + value + "' could not be found");

        }

    }
}
