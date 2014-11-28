using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MvcDemo.Tests
{
    public class AreObjectsEqualHelper
    {
        /// <summary>
        /// Compares the properties of two objects of the same type and returns if all properties are equal.
        /// </summary>
        /// <param name="objectA">The first object to compare.</param>
        /// <param name="objectB">The second object to compre.</param>
        /// <param name="ignoreList">A list of property names to ignore from the comparison.</param>
        /// <returns><c>true</c> if all property values are equal, otherwise <c>false</c>.</returns>
        public static bool IsEqual(object objectA, object objectB, params string[] ignoreList)
        {
            bool result;

            if (objectA != null && objectB != null)
            {
                Type objectType = objectA.GetType();

                result = true; // assume by default they are equal

                foreach (PropertyInfo propertyInfo in objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => p.CanRead && !ignoreList.Contains(p.Name)))
                {
                    object valueA = propertyInfo.GetValue(objectA, null);
                    object valueB = propertyInfo.GetValue(objectB, null);

                    // if it is a primative type, value type or implements IComparable, just directly try and compare the value
                    if (CanDirectlyCompare(propertyInfo.PropertyType))
                    {
                        if (!AreValuesEqual(valueA, valueB))
                        {
                            //Mismatch with property objectType.FullName propertyInfo.Name found.
                            result = false;
                        }
                    }
                    // if it implements IEnumerable, then scan any items
                    else if (typeof(IEnumerable).IsAssignableFrom(propertyInfo.PropertyType))
                    {
                        // null check
                        if (valueA == null && valueB != null || valueA != null && valueB == null)
                        {
                            //Mismatch with property objectType.FullName propertyInfo.Name found.
                            result = false;
                        }
                        else if (valueA != null && valueB != null)
                        {
                            IEnumerable<object> collectionItems1 = ((IEnumerable)valueA).Cast<object>();
                            IEnumerable<object> collectionItems2 = ((IEnumerable)valueB).Cast<object>();

                            int collectionItemsCount1 = collectionItems1.Count();
                            int collectionItemsCount2 = collectionItems2.Count();

                            // check the counts to ensure they match
                            if (collectionItemsCount1 != collectionItemsCount2)
                            {
                                //Collection counts for property objectType.FullName propertyInfo.Name do not match.
                                result = false;
                            }
                            // and if they do, compare each item... this assumes both collections have the same order
                            else
                            {
                                for (int i = 0; i < collectionItemsCount1; i++)
                                {
                                    object collectionItem1 = collectionItems1.ElementAt(i);
                                    object collectionItem2 = collectionItems2.ElementAt(i);
                                    Type collectionItemType = collectionItem1.GetType();

                                    if (CanDirectlyCompare(collectionItemType))
                                    {
                                        if (!AreValuesEqual(collectionItem1, collectionItem2))
                                        {
                                            //Item in property collection objectType.FullName propertyInfo.Name does not match.
                                            result = false;
                                        }
                                    }
                                    else if (!IsEqual(collectionItem1, collectionItem2, ignoreList))
                                    {
                                        //Item in property collection objectType.FullName propertyInfo.Name does not match.
                                        result = false;
                                    }
                                }
                            }
                        }
                    }
                    else if (propertyInfo.PropertyType.IsClass)
                    {
                        if (!IsEqual(propertyInfo.GetValue(objectA, null), propertyInfo.GetValue(objectB, null), ignoreList))
                        {
                            result = false;
                        }
                    }
                    else
                    {
                        // Cannot compare property objectType.FullName propertyInfo.Name.
                        result = false;
                    }
                }
            }
            else
                result = object.Equals(objectA, objectB);

            return result;
        }

        /// <summary>
        /// Determines whether value instances of the specified type can be directly compared.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns>
        ///   <c>true</c> if this value instances of the specified type can be directly compared; otherwise, <c>false</c>.
        /// </returns>
        private static bool CanDirectlyCompare(Type type)
        {
            return typeof(IComparable).IsAssignableFrom(type) || type.IsPrimitive || type.IsValueType;
        }

        /// <summary>
        /// Compares two values and returns if they are the same.
        /// </summary>
        /// <param name="valueA">The first value to compare.</param>
        /// <param name="valueB">The second value to compare.</param>
        /// <returns><c>true</c> if both values match, otherwise <c>false</c>.</returns>
        private static bool AreValuesEqual(object valueA, object valueB)
        {
            bool result;
            var selfValueComparer = valueA as IComparable;

            if (valueA == null && valueB != null || valueA != null && valueB == null)
                result = false; // one of the values is null
            else if (selfValueComparer != null && selfValueComparer.CompareTo(valueB) != 0)
                result = false; // the comparison using IComparable failed
            else if (!object.Equals(valueA, valueB))
                result = false; // the comparison using Equals failed
            else
                result = true; // match

            return result;
        }

    }
}
