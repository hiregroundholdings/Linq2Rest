// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GeneralExtensions.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2014
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the GeneralExtensions type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace LinqCovertTools
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using System.Runtime.CompilerServices;
    using System.Text;

    internal static class GeneralExtensions
    {
        private static readonly ConcurrentDictionary<Type, bool> KnownAnonymousTypes = new ConcurrentDictionary<Type, bool>();

        public static bool IsAnonymousType(this Type type)
        {
            if (type is null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return KnownAnonymousTypes.GetOrAdd(
                type,
                t => Attribute.IsDefined(t, typeof(CompilerGeneratedAttribute), false)
                        && t.IsGenericType
                        && t.Name.Contains("AnonymousType") && (t.Name.StartsWith("<>") || t.Name.StartsWith("VB$"))
                        && (t.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic);
        }

        public static string Capitalize(this string input)
        {
            return char.ToUpperInvariant(input[0]) + input.Substring(1);
        }

        public static Stream ToStream(this string input)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(input ?? string.Empty));
        }

        public static IEnumerable<T> Replace<T>(this IEnumerable<T> items, Func<T, bool> predicate, T replacement)
        {
            return items.Select(item => predicate(item) ? replacement : item);
        }

        public static Tuple<Type, Expression?> CreateMemberExpression(this IMemberNameResolver memberNameResolver, ParameterExpression parameter, IEnumerable<string> propertyChain, Type parentType, Expression? propertyExpression)
        {
            foreach (string propertyName in propertyChain)
            {
                string name = propertyName;
                MemberInfo member = memberNameResolver.ResolveAlias(parentType, name);
                if (member is not null)
                {
                    parentType = GetMemberType(member);
                    propertyExpression = propertyExpression is null
                                             ? Expression.MakeMemberAccess(parameter, member)
                                             : Expression.MakeMemberAccess(propertyExpression, member);
                }
            }

            return new Tuple<Type, Expression?>(parentType, propertyExpression);
        }

        public static Tuple<Type, string> GetNameFromAlias(this IMemberNameResolver memberNameResolver, MemberInfo alias, Type sourceType)
        {
            var source = sourceType.GetMembers()
                .Select(x => new { Original = x, Name = memberNameResolver.ResolveName(x) })
                .FirstOrDefault(x => x.Original.Name == alias.Name);

            return source != null
                       ? new Tuple<Type, string>(GetMemberType(source.Original), source.Name)
                       : new Tuple<Type, string>(GetMemberType(alias), alias.Name);
        }

        public static IOrderedQueryable<T> OrderBy<T>(this IQueryable<T> source, Expression keySelector)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector is null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            var propertyType = keySelector.GetType().GetGenericArguments()[0].GetGenericArguments()[1];
            var orderbyMethod = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(x => x.Name == "OrderBy" && x.GetParameters().Length == 2);



            orderbyMethod = orderbyMethod.MakeGenericMethod(typeof(T), propertyType);

            return (IOrderedQueryable<T>)orderbyMethod.Invoke(null, new object[] { source, keySelector });
        }

        public static IOrderedQueryable<T> OrderByDescending<T>(this IQueryable<T> source, Expression keySelector)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector is null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            var propertyType = keySelector.GetType().GetGenericArguments()[0].GetGenericArguments()[1];
            var orderbyMethod = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(x => x.Name == "OrderByDescending" && x.GetParameters().Length == 2);



            orderbyMethod = orderbyMethod.MakeGenericMethod(typeof(T), propertyType);

            return (IOrderedQueryable<T>)orderbyMethod.Invoke(null, new object[] { source, keySelector });
        }

        public static IOrderedQueryable<T> ThenBy<T>(this IOrderedQueryable<T> source, Expression keySelector)
        {
            if (source is null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector is null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            var propertyType = keySelector.GetType().GetGenericArguments()[0].GetGenericArguments()[1];
            var orderbyMethod = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(x => x.Name == "ThenBy" && x.GetParameters().Length == 2);



            orderbyMethod = orderbyMethod.MakeGenericMethod(typeof(T), propertyType);

            return (IOrderedQueryable<T>)orderbyMethod.Invoke(null, new object[] { source, keySelector });
        }

        public static IOrderedQueryable<T> ThenByDescending<T>(this IOrderedQueryable<T> source, Expression keySelector)
        {



            var propertyType = keySelector.GetType().GetGenericArguments()[0].GetGenericArguments()[1];
            var orderbyMethod = typeof(Queryable).GetMethods(BindingFlags.Public | BindingFlags.Static).FirstOrDefault(x => x.Name == "ThenByDescending" && x.GetParameters().Length == 2);



            orderbyMethod = orderbyMethod.MakeGenericMethod(typeof(T), propertyType);

            return (IOrderedQueryable<T>)orderbyMethod.Invoke(null, new object[] { source, keySelector });
        }

        private static Type GetMemberType(MemberInfo member)
        {


            switch (member.MemberType)
            {
                case MemberTypes.Field:
                    return ((FieldInfo)member).FieldType;
                case MemberTypes.Property:
                    return ((PropertyInfo)member).PropertyType;
                case MemberTypes.Method:
                    return ((MethodInfo)member).ReturnType;
                default:
                    throw new InvalidOperationException(member.MemberType + " is not resolvable");
            }
        }
    }
}