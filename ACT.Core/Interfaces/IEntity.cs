using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ACT.Core.Models;

namespace ACT.Core.Interfaces
{
    public interface IEntity<T> where T : class
    {
        int Total();

        List<T> List();

        string MaxString( string column );

        string MinString( string column );

        DateTime? MinDateTime( string column );

        int Total( PagingModel pm, CustomSearchModel csm );

        List<T> List( PagingModel pm, CustomSearchModel csm );

        T GetById( int Id );

        T Create( T item, bool track );

        T Update( T item );

        bool Delete( T Item );
    }
}
