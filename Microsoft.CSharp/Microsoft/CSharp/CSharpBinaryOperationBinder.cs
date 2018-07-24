﻿// ==++==
//
//   Copyright (c) Microsoft Corporation.  All rights reserved.
//
// ==--==

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq.Expressions;

namespace Microsoft.CSharp.RuntimeBinder
{
    /// <summary>
    /// Represents a dynamic binary operation in C#, providing the binding semantics and the details about the operation. 
    /// Instances of this class are generated by the C# compiler.
    /// </summary>
    internal sealed class CSharpBinaryOperationBinder : BinaryOperationBinder
    {
        internal bool IsChecked { get { return m_isChecked; } }
        private bool m_isChecked;

        internal bool IsLogicalOperation { get { return (m_binopFlags & CSharpBinaryOperationFlags.LogicalOperation) != 0; } }
        private CSharpBinaryOperationFlags m_binopFlags;

        internal Type CallingContext { get { return m_callingContext; } }
        private Type m_callingContext;

        internal IList<CSharpArgumentInfo> ArgumentInfo { get { return m_argumentInfo.AsReadOnly(); } }
        private List<CSharpArgumentInfo> m_argumentInfo;

        private RuntimeBinder m_binder;

        //////////////////////////////////////////////////////////////////////

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpBinaryOperationBinder"/> class.
        /// </summary>
        /// <param name="operation">The binary operation kind.</param>
        /// <param name="isChecked">True if the operation is defined in a checked context; otherwise false.</param>        
        /// <param name="binaryOperationFlags">The flags associated with this binary operation.</param>
        /// <param name="argumentInfo">The sequence of <see cref="CSharpArgumentInfo"/> instances for the arguments to this operation.</param>
        public CSharpBinaryOperationBinder(
            ExpressionType operation,
            bool isChecked,
            CSharpBinaryOperationFlags binaryOperationFlags,
            Type callingContext,
            IEnumerable<CSharpArgumentInfo> argumentInfo) :
            base(operation)
        {
            m_isChecked = isChecked;
            m_binopFlags = binaryOperationFlags;
            m_callingContext = callingContext;
            m_argumentInfo = BinderHelper.ToList(argumentInfo);
            Debug.Assert(m_argumentInfo.Count == 2);
            m_binder = RuntimeBinder.GetInstance();
        }

        /// <summary>
        /// Performs the binding of the binary dynamic operation if the target dynamic object cannot bind.
        /// </summary>
        /// <param name="target">The target of the dynamic binary operation.</param>
        /// <param name="arg">The right hand side operand of the dynamic binary operation.</param>
        /// <param name="errorSuggestion">The binding result in case the binding fails, or null.</param>
        /// <returns>The <see cref="DynamicMetaObject"/> representing the result of the binding.</returns>
        public sealed override DynamicMetaObject FallbackBinaryOperation(DynamicMetaObject target, DynamicMetaObject arg, DynamicMetaObject errorSuggestion)
        {
            return BinderHelper.Bind(this, m_binder, BinderHelper.Cons(target, null, arg), m_argumentInfo, errorSuggestion);
        }
    }
}