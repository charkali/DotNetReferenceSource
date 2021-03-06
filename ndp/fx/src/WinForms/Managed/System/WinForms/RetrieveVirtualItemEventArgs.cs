using System;
using System.Windows.Forms;

namespace System.Windows.Forms {
    /// <include file='doc\RetrieveVirtualItemEventArgs.uex' path='docs/doc[@for="RetrieveVirtualItemEventArgs"]/*' />
    public class RetrieveVirtualItemEventArgs : EventArgs {
        private int itemIndex;
        private ListViewItem item;

        /// <include file='doc\RetrieveVirtualItemEventArgs.uex' path='docs/doc[@for="RetrieveVirtualItemEventArgs.RetrieveVirtualItemEventArgs"]/*' />
        public RetrieveVirtualItemEventArgs(int itemIndex) {
            this.itemIndex = itemIndex;
        }

        /// <include file='doc\RetrieveVirtualItemEventArgs.uex' path='docs/doc[@for="RetrieveVirtualItemEventArgs.ItemIndex"]/*' />
        public int ItemIndex {
            get {
                return itemIndex;
            }
        }

        /// <include file='doc\RetrieveVirtualItemEventArgs.uex' path='docs/doc[@for="RetrieveVirtualItemEventArgs.Item"]/*' />
        public ListViewItem Item {
            get {
                return item;
            }
            set {
                this.item = value;
            }
        }
    }
}
