using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;

namespace CoCo.UI.ViewModels
{
    public static class Extensions
    {
        public static ListCollectionView GetDefaultListView<T>(this ObservableCollection<T> collection, IComparer comparer = null)
        {
            /// NOTE: avoid redundant creation of <see cref="ListCollectionView"/>
            if (!(CollectionViewSource.GetDefaultView(collection) is ListCollectionView listView))
            {
                listView = new ListCollectionView(collection);
            }
            listView.CustomSort = comparer ?? StringComparer.Ordinal;
            return listView;
        }
    }
}