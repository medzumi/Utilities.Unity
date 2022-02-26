using System;

namespace ApplicationScripts.ViewModel
{
    public static class TypeExtensions
    {
        public static bool IsSubclassDeep(this Type type, Type parentType)
        {
            if (type == parentType)
                return true;
            
            while (type != null)
            {
                if (type.IsSubclassOf(parentType))
                    return true;
                type = type.BaseType;
            }

            return false;
        }

        public static bool TryGetGenericTypeOfDefinition(this Type type, Type genericTypeDefinition,
            out Type generictype)
        {
            generictype = null;
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == genericTypeDefinition)
                {
                    generictype = type;
                    return true;
                }
                type = type.BaseType;
            }

            return false;
        }
        
        public static bool IsSubclassOfGenericTypeDefinition(this Type t, Type genericTypeDefinition)
        {
            if (!genericTypeDefinition.IsGenericTypeDefinition)
            {
                throw new Exception("genericTypeDefinition parameter isn't generic type definition");
            }
            if (t.IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition)
            {
                return true;
            }
            else
            {
                t = t.BaseType;
                while (t !=null)
                {
                    if (t.IsGenericType && t.GetGenericTypeDefinition() == genericTypeDefinition)
                        return true;

                    t = t.BaseType;
                }
            }

            return false;
        }
    }
}