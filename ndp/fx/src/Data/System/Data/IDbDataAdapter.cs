//------------------------------------------------------------------------------
// <copyright file="IDbDataAdapter.cs" company="Microsoft">
//      Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>
// <owner current="true" primary="true">[....]</owner>
// <owner current="true" primary="false">[....]</owner>
//------------------------------------------------------------------------------

namespace System.Data {

    public interface IDbDataAdapter : IDataAdapter {

        IDbCommand SelectCommand {
            get;
            set;
        }

        IDbCommand InsertCommand {
            get;
            set;
        }

        IDbCommand UpdateCommand {
            get;
            set;
        }

        IDbCommand DeleteCommand {
            get;
            set;
        }
    }
}
