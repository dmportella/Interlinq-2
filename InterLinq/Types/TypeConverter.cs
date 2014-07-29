using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using InterLinq.Types.Anonymous;

namespace InterLinq.Types
{
    /// <summary>
    /// The <see cref="TypeConverter"/> is a helper class providing
    /// several static methods to convert <see cref="AnonymousObject"/> to
    /// C# Anonymous Types and back.
    /// </summary>
#if SILVERLIGHT
    public class TypeConverter
#else
    internal class TypeConverter
#endif
    {

        #region Convert AnonymousObject To C# Anonymous Type

        /// <summary>
        /// Converts an <see langword="object"/> into a target <see cref="Type"/>.
        /// </summary>
        /// <param name="wantedType">Target <see cref="Type"/>.</param>
        /// <param name="objectToConvert"><see langword="object"/> to convert.</param>
        /// <returns>Returns the converted <see langword="object"/>.</returns>
        public static object ConvertFromSerializable(Type wantedType, object objectToConvert)
        {
            if (objectToConvert == null)
            {
                return null;
            }
            if (wantedType.IsIGrouping() && objectToConvert is InterLinqGroupingBase)
            {
#if !NETFX_CORE
                Type[] genericType = objectToConvert.GetType().GetGenericArguments();
#else
                Type[] genericType = objectToConvert.GetType().GetTypeInfo().GenericTypeArguments;
#endif
#if !SILVERLIGHT
    			MethodInfo method = typeof(TypeConverter).GetMethod("ConvertFromInterLinqGrouping", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(genericType);
#else
#if !NETFX_CORE
                MethodInfo method = typeof(TypeConverter).GetMethod("ConvertFromInterLinqGrouping", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(genericType);
#else
                MethodInfo method = typeof(TypeConverter).GetTypeInfo().GetDeclaredMethod("ConvertFromInterLinqGrouping").MakeGenericMethod(genericType);

#endif
#endif

                return method.Invoke(null, new[] { wantedType, objectToConvert });
            }

            if (wantedType.IsIGroupingArray() && objectToConvert is InterLinqGroupingBase)
            {
#if !NETFX_CORE
                Type[] genericType = objectToConvert.GetType().GetGenericArguments();
#else
                Type[] genericType = objectToConvert.GetType().GetTypeInfo().GenericTypeArguments;
#endif
#if !SILVERLIGHT
                MethodInfo method = typeof(TypeConverter).GetMethod("ConvertFromInterLinqGroupingArray", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(genericType);
#else
#if !NETFX_CORE
                MethodInfo method = typeof(TypeConverter).GetMethod("ConvertFromInterLinqGroupingArray", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(genericType);
#else
                MethodInfo method = typeof(TypeConverter).GetTypeInfo().GetDeclaredMethod("ConvertFromInterLinqGroupingArray").MakeGenericMethod(genericType);
#endif
#endif

                return method.Invoke(null, new[] { wantedType, objectToConvert });
            }

            Type wantedElementType = InterLinqTypeSystem.FindIEnumerable(wantedType);
#if !NETFX_CORE
            if (wantedElementType != null && wantedElementType.GetGenericArguments()[0].IsAnonymous())
#else
            if (wantedElementType != null && wantedElementType.GetTypeInfo().GenericTypeArguments[0].IsAnonymous())
#endif
            {
                Type typeOfObject = objectToConvert.GetType();
                Type elementType = InterLinqTypeSystem.FindIEnumerable(typeOfObject);
#if !NETFX_CORE
                if (elementType != null && elementType.GetGenericArguments()[0] == typeof(AnonymousObject))
#else
                if (elementType != null && elementType.GetTypeInfo().GenericTypeArguments[0] == typeof(AnonymousObject))
#endif
                {
#if !SILVERLIGHT
                    MethodInfo method = typeof(TypeConverter).GetMethod("ConvertFromSerializableCollection", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(wantedElementType.GetGenericArguments()[0]);
#else
#if !NETFX_CORE
                    MethodInfo method = typeof(TypeConverter).GetMethod("ConvertFromSerializableCollection", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(wantedElementType.GetGenericArguments()[0]);
#else
                    MethodInfo method = typeof(TypeConverter).GetTypeInfo().GetDeclaredMethod("ConvertFromSerializableCollection").MakeGenericMethod(wantedElementType.GetTypeInfo().GenericTypeArguments[0]);
#endif
#endif
                    return method.Invoke(null, new[] { objectToConvert });
                }
            }
            if (wantedType.IsAnonymous() && objectToConvert is AnonymousObject)
            {
                AnonymousObject dynamicObject = (AnonymousObject)objectToConvert;
                List<object> properties = new List<object>();
#if !NETFX_CORE
                ConstructorInfo[] constructors = wantedType.GetConstructors();
#else
                ConstructorInfo[] constructors = wantedType.GetTypeInfo().DeclaredConstructors.ToArray();
#endif
                if (constructors.Length != 1)
                {
                    throw new Exception("Usualy, anonymous types have just one constructor.");
                }
                ConstructorInfo constructor = constructors[0];
                foreach (ParameterInfo parameter in constructor.GetParameters())
                {
                    object propertyValue = null;
                    bool propertyHasBeenSet = false;
                    foreach (AnonymousProperty dynProperty in dynamicObject.Properties)
                    {
                        if (dynProperty.Name == parameter.Name)
                        {
                            propertyValue = dynProperty.Value;
                            propertyHasBeenSet = true;
                            break;
                        }
                    }
                    if (!propertyHasBeenSet)
                    {
                        throw new Exception(string.Format("Property {0} could not be found in the dynamic object.", parameter.Name));
                    }
                    properties.Add(ConvertFromSerializable(parameter.ParameterType, propertyValue));
                }
                return constructor.Invoke(properties.ToArray());
            }
            return objectToConvert;
        }

#if !SILVERLIGHT
        private static object ConvertFromInterLinqGrouping<TKey, TElement>(Type wantedType, InterLinqGrouping<TKey, TElement> grouping)
        {
            Type[] genericArguments = wantedType.GetGenericArguments();
            object key = ConvertFromSerializable(genericArguments[0], grouping.Key);

            MethodInfo method = typeof(TypeConverter).GetMethod("ConvertFromSerializableCollection", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(genericArguments[1]);
            object elements = method.Invoke(null, new object[] { grouping });

            //object elements = ConvertFromSerializableCollection<TElement>( typeof( IEnumerable<> ).MakeGenericType( genericArguments[1] ),  );
            Type elementType = InterLinqTypeSystem.FindIEnumerable(elements.GetType());
            if (elementType == null)
            {
                throw new Exception("ElementType could not be found.");
            }
            Type[] genericTypes = new[] { key.GetType(), elementType.GetGenericArguments()[0] };
            InterLinqGroupingBase newGrouping = (InterLinqGroupingBase)Activator.CreateInstance(typeof(InterLinqGrouping<,>).MakeGenericType(genericTypes));
            newGrouping.SetKey(key);
            newGrouping.SetElements(elements);
            return newGrouping;
        }

        private static object ConvertFromInterLinqGroupingArray<TKey, TElement>(Type wantedType, InterLinqGrouping<TKey, TElement>[] grouping)
        {
            var retVal = new List<InterLinqGroupingBase>();
            var tp = wantedType.GetElementType();
            foreach (var interLinqGrouping in grouping)
            {
                retVal.Add((InterLinqGroupingBase) ConvertFromInterLinqGrouping(tp, interLinqGrouping));

            }
            return retVal.ToArray();
        }

		/// <summary>
        /// Converts each element of an <see cref="IEnumerable"/> 
        /// into a target <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">Target <see cref="Type"/>.</typeparam>
        /// <param name="enumerable"><see cref="IEnumerable"/>.</param>
        /// <returns>Returns the converted <see cref="IEnumerable"/>.</returns>
        private static IEnumerable ConvertFromSerializableCollection<T>(IEnumerable enumerable)
        {
            Type enumerableType = typeof(List<>).MakeGenericType(typeof(T));
            IEnumerable newList = (IEnumerable)Activator.CreateInstance(enumerableType);
            MethodInfo addMethod = enumerableType.GetMethod("Add");
            foreach (object item in enumerable)
            {
                addMethod.Invoke(newList, new[] { ConvertFromSerializable(typeof(T), item) });
            }
            return newList;
        }
#else
		public static object ConvertFromInterLinqGrouping<TKey, TElement>(Type wantedType, InterLinqGrouping<TKey, TElement> grouping)
        {
#if !NETFX_CORE
            Type[] genericArguments = wantedType.GetGenericArguments();
#else
            Type[] genericArguments = wantedType.GetTypeInfo().GenericTypeArguments;
#endif
            object key = ConvertFromSerializable(genericArguments[0], grouping.Key);

#if !NETFX_CORE
            MethodInfo method = typeof(TypeConverter).GetMethod("ConvertFromSerializableCollection", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(genericArguments[1]);
#else
            MethodInfo method = typeof(TypeConverter).GetTypeInfo().GetDeclaredMethod("ConvertFromSerializableCollection").MakeGenericMethod(genericArguments[1]);
#endif
            object elements = method.Invoke(null, new object[] { grouping });

            //object elements = ConvertFromSerializableCollection<TElement>( typeof( IEnumerable<> ).MakeGenericType( genericArguments[1] ),  );
            Type elementType = InterLinqTypeSystem.FindIEnumerable(elements.GetType());
            if (elementType == null)
            {
                throw new Exception("ElementType could not be found.");
            }
#if !NETFX_CORE
            Type[] genericTypes = new[] { key.GetType(), elementType.GetGenericArguments()[0] };
#else
            Type[] genericTypes = new[] { key.GetType(), elementType.GetTypeInfo().GenericTypeArguments[0] };
#endif
            InterLinqGroupingBase newGrouping = (InterLinqGroupingBase)Activator.CreateInstance(typeof(InterLinqGrouping<,>).MakeGenericType(genericTypes));
            newGrouping.SetKey(key);
            newGrouping.SetElements(elements);
            return newGrouping;
        }

        public static object ConvertFromInterLinqGroupingArray<TKey, TElement>(Type wantedType, InterLinqGrouping<TKey, TElement>[] grouping)
        {
            var retVal = new List<InterLinqGroupingBase>();
            var tp = wantedType.GetElementType();
            foreach (var interLinqGrouping in grouping)
            {
                retVal.Add((InterLinqGroupingBase) ConvertFromInterLinqGrouping(tp, interLinqGrouping));

            }
            return retVal.ToArray();
        }

		/// <summary>
        /// Converts each element of an <see cref="IEnumerable"/> 
        /// into a target <see cref="Type"/>.
        /// </summary>
        /// <typeparam name="T">Target <see cref="Type"/>.</typeparam>
        /// <param name="enumerable"><see cref="IEnumerable"/>.</param>
        /// <returns>Returns the converted <see cref="IEnumerable"/>.</returns>
        public static IEnumerable ConvertFromSerializableCollection<T>(IEnumerable enumerable)
        {
            Type enumerableType = typeof(List<>).MakeGenericType(typeof(T));
            IEnumerable newList = (IEnumerable)Activator.CreateInstance(enumerableType);
#if !NETFX_CORE
            MethodInfo addMethod = enumerableType.GetMethod("Add");
#else
            MethodInfo addMethod = enumerableType.GetTypeInfo().GetDeclaredMethod("Add");
#endif
            foreach (object item in enumerable)
            {
                addMethod.Invoke(newList, new[] { ConvertFromSerializable(typeof(T), item) });
            }
            return newList;
        }
#endif
        #endregion

        #region Convert C# Anonymous Type to AnonymousObject

        /// <summary>
        /// Converts an object to an <see cref="AnonymousObject"/> 
        /// or an <see cref="IEnumerable{AnonymousObject}"/>.
        /// </summary>
        /// <param name="objectToConvert"><see langword="object"/> to convert.</param>
        /// <returns>Returns the converted <see langword="object"/>.</returns>
        public static object ConvertToSerializable(object objectToConvert)
        {
            if (objectToConvert == null)
            {
                return null;
            }

            Type typeOfObject = objectToConvert.GetType();
            Type elementType = InterLinqTypeSystem.FindIEnumerable(typeOfObject);

            // Handle "IGrouping<TKey, TElement>"
            if (typeOfObject.IsIGrouping())
            {
#if !NETFX_CORE
                Type[] genericType = typeOfObject.GetGenericArguments();
#else
                Type[] genericType = typeOfObject.GetTypeInfo().GenericTypeArguments;
#endif
#if !SILVERLIGHT
                MethodInfo method = typeof(TypeConverter).GetMethod("ConvertToInterLinqGrouping", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(genericType);
#else
#if !NETFX_CORE
                MethodInfo method = typeof(TypeConverter).GetMethod("ConvertToInterLinqGrouping", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(genericType);
#else
                MethodInfo method = typeof(TypeConverter).GetTypeInfo().GetDeclaredMethod("ConvertToInterLinqGrouping").MakeGenericMethod(genericType);

#endif
#endif
                return method.Invoke(null, new[] { objectToConvert });
            }

            // Handle "IGrouping<TKey, TElement>[]"
            if (typeOfObject.IsIGroupingArray())
            {
#if !NETFX_CORE
                Type[] genericType = typeOfObject.GetGenericArguments();
#else
                Type[] genericType = typeOfObject.GetTypeInfo().GenericTypeArguments;
#endif
#if !SILVERLIGHT
                MethodInfo method = typeof(TypeConverter).GetMethod("ConvertToInterLinqGroupingArray", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(genericType);
#else
#if !NETFX_CORE
                MethodInfo method = typeof(TypeConverter).GetMethod("ConvertToInterLinqGroupingArray", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(genericType);
#else
                MethodInfo method = typeof(TypeConverter).GetTypeInfo().GetDeclaredMethod("ConvertToInterLinqGroupingArray").MakeGenericMethod(genericType);
#endif
#endif
                return method.Invoke(null, new[] { objectToConvert });
            }

            // Handle "IEnumerable<AnonymousType>" / "IEnumerator<T>"
#if !NETFX_CORE
            if (elementType != null && elementType.GetGenericArguments()[0].IsAnonymous() || typeOfObject.IsEnumerator())
#else
            if (elementType != null && elementType.GetTypeInfo().GenericTypeArguments[0].IsAnonymous() || typeOfObject.IsEnumerator())
#endif
            {
#if !SILVERLIGHT
                MethodInfo method = typeof(TypeConverter).GetMethod("ConvertToSerializableCollection", BindingFlags.NonPublic | BindingFlags.Static).MakeGenericMethod(elementType.GetGenericArguments()[0]);
#else
#if !NETFX_CORE
				MethodInfo method = typeof(TypeConverter).GetMethod("ConvertToSerializableCollection", BindingFlags.Public | BindingFlags.Static).MakeGenericMethod(elementType.GetGenericArguments()[0]);
#else
                MethodInfo method = typeof(TypeConverter).GetTypeInfo().GetDeclaredMethod("ConvertToSerializableCollection").MakeGenericMethod(elementType.GetTypeInfo().GenericTypeArguments[0]);
#endif
#endif
                return method.Invoke(null, new[] { objectToConvert });
            }
            // Handle "AnonymousType"
            if (typeOfObject.IsAnonymous())
            {
                AnonymousObject newObject = new AnonymousObject();
#if !NETFX_CORE
                foreach (PropertyInfo property in typeOfObject.GetProperties(BindingFlags.Instance | BindingFlags.GetProperty | BindingFlags.Public))
#else
                foreach (PropertyInfo property in typeOfObject.GetTypeInfo().DeclaredProperties)
#endif
                {
                    object objectValue = ConvertToSerializable(property.GetValue(objectToConvert, new object[] { }));
                    newObject.Properties.Add(new AnonymousProperty(property.Name, objectValue));
                }
                return newObject;
            }

            return objectToConvert;
        }

#if !SILVERLIGHT
        private static object ConvertToInterLinqGrouping<TKey, TElement>(IGrouping<TKey, TElement> grouping)
        {
            object key = ConvertToSerializable(grouping.Key);
            object elements = ConvertToSerializableCollection<TElement>(grouping);
            Type elementType = InterLinqTypeSystem.FindIEnumerable(elements.GetType());
            if (elementType == null)
            {
                throw new Exception("ElementType could not be found.");
            }
            Type[] genericTypes = new Type[] { key.GetType(), elementType.GetGenericArguments()[0] };
            InterLinqGroupingBase newGrouping = (InterLinqGroupingBase)Activator.CreateInstance(typeof(InterLinqGrouping<,>).MakeGenericType(genericTypes));
            newGrouping.SetKey(key);
            newGrouping.SetElements(elements);
            return newGrouping;
        }

        private static object ConvertToInterLinqGroupingArray<TKey, TElement>(IGrouping<TKey, TElement>[] grouping)
        {
            var retVal = new List<InterLinqGrouping<TKey, TElement>>();
            foreach (var g in grouping)
            {
                retVal.Add((InterLinqGrouping<TKey, TElement>)ConvertToInterLinqGrouping(g));
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Converts each element of an <see cref="IEnumerable"/> to 
        /// an <see cref="IEnumerable{AnonymousObject}"/> 
        /// </summary>
        /// <typeparam name="T">Target <see cref="Type"/>.</typeparam>
        /// <param name="enumerable"><see cref="IEnumerable"/>.</param>
        /// <returns>Returns the converted <see cref="IEnumerable"/>.</returns>
        private static IEnumerable ConvertToSerializableCollection<T>(IEnumerable enumerable)
        {
            Type typeToEnumerate = typeof(T);
            if (typeToEnumerate.IsAnonymous())
            {
                typeToEnumerate = typeof(AnonymousObject);
            }
            Type enumerableType = typeof(List<>).MakeGenericType(typeToEnumerate);
            IEnumerable newList = (IEnumerable)Activator.CreateInstance(enumerableType);
            MethodInfo addMethod = enumerableType.GetMethod("Add");
            foreach (object item in enumerable)
            {
                addMethod.Invoke(newList, new[] { ConvertToSerializable(item) });
            }
            return newList;
        }
#else
		public static object ConvertToInterLinqGrouping<TKey, TElement>(IGrouping<TKey, TElement> grouping)
        {
            object key = ConvertToSerializable(grouping.Key);
            object elements = ConvertToSerializableCollection<TElement>(grouping);
            Type elementType = InterLinqTypeSystem.FindIEnumerable(elements.GetType());
            if (elementType == null)
            {
                throw new Exception("ElementType could not be found.");
            }
#if !NETFX_CORE
            Type[] genericTypes = new Type[] { key.GetType(), elementType.GetGenericArguments()[0] };
#else
            Type[] genericTypes = new Type[] { key.GetType(), elementType.GetTypeInfo().GenericTypeArguments[0] };
#endif
            InterLinqGroupingBase newGrouping = (InterLinqGroupingBase)Activator.CreateInstance(typeof(InterLinqGrouping<,>).MakeGenericType(genericTypes));
            newGrouping.SetKey(key);
            newGrouping.SetElements(elements);
            return newGrouping;
        }

        public static object ConvertToInterLinqGroupingArray<TKey, TElement>(IGrouping<TKey, TElement>[] grouping)
        {
            var retVal = new List<InterLinqGrouping<TKey, TElement>>();
            foreach (var g in grouping)
            {
                retVal.Add((InterLinqGrouping<TKey, TElement>) ConvertToInterLinqGrouping(g));
            }

            return retVal.ToArray();
        }

        /// <summary>
        /// Converts each element of an <see cref="IEnumerable"/> to 
        /// an <see cref="IEnumerable{AnonymousObject}"/> 
        /// </summary>
        /// <typeparam name="T">Target <see cref="Type"/>.</typeparam>
        /// <param name="enumerable"><see cref="IEnumerable"/>.</param>
        /// <returns>Returns the converted <see cref="IEnumerable"/>.</returns>
        public static IEnumerable ConvertToSerializableCollection<T>(IEnumerable enumerable)
        {
            Type typeToEnumerate = typeof(T);
            if (typeToEnumerate.IsAnonymous())
            {
                typeToEnumerate = typeof(AnonymousObject);
            }
            Type enumerableType = typeof(List<>).MakeGenericType(typeToEnumerate);
            IEnumerable newList = (IEnumerable)Activator.CreateInstance(enumerableType);
#if !NETFX_CORE
            MethodInfo addMethod = enumerableType.GetMethod("Add");
#else
            MethodInfo addMethod = enumerableType.GetTypeInfo().GetDeclaredMethod("Add");
#endif
            foreach (object item in enumerable)
            {
                addMethod.Invoke(newList, new[] { ConvertToSerializable(item) });
            }
            return newList;
        }
#endif

#endregion

        }
    }
