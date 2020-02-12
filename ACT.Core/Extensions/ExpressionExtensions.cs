using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace System.Linq.Expressions {



    public static class ExpressionExtensions {


        public static MemberInfo GetMemberInfo<T, V>(this Expression<Func<T, V>> expression) {
            MemberExpression memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null) {
                try {
                    memberExpression = (expression.Body as UnaryExpression).Operand as MemberExpression;
                } catch { }
            }

            if (memberExpression == null)
                throw new InvalidOperationException("Expression must be a member expression.");

            return memberExpression.Member;
        }

        public static MemberInfo GetMemberInfo<T, V>(this Expression<Func<T, V>> expression, object objectInstance, FindMode findMode) {

            MemberInfo member = expression.GetMemberInfo();

            if ((member.DeclaringType.IsInterface) && (findMode == FindMode.FindClass) && (objectInstance != null)) {

                Type t = objectInstance.GetType();
                if (t != member.DeclaringType) {
                    MemberInfo newMember = t.GetProperty(member.Name);
                    if (newMember != null) {
                        member = newMember;
                    }
                }

            }

            return member;
            
            
        }

        public static bool IsRequired<T, V>(this Expression<Func<T, V>> expression) {
            var memberExpression = expression.Body as MemberExpression;

            if (memberExpression == null) {
                try {
                    memberExpression = (expression.Body as UnaryExpression).Operand as MemberExpression;
                } catch { }
            }

            if (memberExpression == null)
                throw new InvalidOperationException("Expression must be a member expression");

            return memberExpression.Member.GetAttribute<RequiredAttribute>() != null;
        }


    }

    public enum FindMode {
        AcceptInterface,
        FindClass
    }
}