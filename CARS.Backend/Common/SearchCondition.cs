using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CARS.Backend.Common
{
    public class SearchCondition
    {
        private string searchKey;
        private string searchValue;
        private SearchComparator comparator;
        private SearchType comparaType;

        public string SearchKey
        {
            get { return searchKey; }
        }

        public string SearchValue
        {
            get { return searchValue; }
        }

        public SearchComparator Comparator
        {
            get { return comparator; }
        }

        public SearchType ComparaType
        {
            get { return comparaType; }
        }

        private SearchCondition(string searchKey, string searchValue, SearchComparator comparator, SearchType comparaType)
        {
            this.searchKey = searchKey;
            this.searchValue = searchValue;
            this.comparator = comparator;
            this.comparaType = comparaType;
        }

        public static SearchCondition CreateSearchCondition(string searchKey, string searchValue, SearchComparator comparator, SearchType comparaType)
        {
            return new SearchCondition(searchKey, searchValue, comparator, comparaType);
        }
    }
}
