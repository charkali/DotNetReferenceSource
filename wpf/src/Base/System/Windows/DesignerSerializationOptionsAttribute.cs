//---------------------------------------------------------------------------
//
// File: DesignerSerializationOptionsAttribute.cs
//
// Description:
//   Specifies the serialization flags per property
//
// Copyright (C) 2003 by Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.ComponentModel;
using MS.Internal.WindowsBase;

namespace System.Windows.Markup
{
    /// <summary>
    ///     Specifies the serialization flags per property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Method, AllowMultiple = false)]
    public sealed class DesignerSerializationOptionsAttribute : Attribute
    {
        #region Construction
        
        /// <summary>
        ///     Constructor for DesignerSerializationOptionsAttribute
        /// </summary>
        public DesignerSerializationOptionsAttribute(DesignerSerializationOptions designerSerializationOptions)
        {
            if (DesignerSerializationOptions.SerializeAsAttribute == designerSerializationOptions)
            {
                _designerSerializationOptions = designerSerializationOptions;
            }
            else
            {
                throw new InvalidEnumArgumentException(SR.Get(SRID.Enum_Invalid, "DesignerSerializationOptions"));
            }
        }

        #endregion Construction

        #region Properties

        /// <summary>
        ///     DesignerSerializationOptions
        /// </summary>
        public DesignerSerializationOptions DesignerSerializationOptions
        {
            get { return _designerSerializationOptions; }
        }
        
        #endregion Properties

        #region Data

        DesignerSerializationOptions _designerSerializationOptions;
        
        #endregion Data
    }
}

