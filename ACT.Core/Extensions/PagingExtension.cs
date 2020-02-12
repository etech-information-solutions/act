using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ACT.Data.Models;

namespace System
{
    public class PagingExtension
    {
        #region Properties

        public int Total { get; set; }

        public int Start { get; set; }

        public int Middle { get; set; }

        public object Items { get; set; }

        public object GroupObject { get; set; }

        #endregion

        #region Navigation Configuration

        public bool ShowNext { get; set; }

        public bool ShowPrev { get; set; }

        public int NSkip { get; set; }

        public int NPage { get; set; }

        public int PSkip { get; set; }

        public int PPage { get; set; }

        #endregion

        public static PagingExtension Create( object items, int total, int skip, int take, int page )
        {
            List<object> obj = ( ( IEnumerable ) items ).Cast<object>().ToList();

            int skpN = 0, skpP = 0, pgN = 0, pgP = 0;

            int start = ( skip == 0 ) ? ( ( total <= 0 ) ? 0 : 1 ) : ( skip + 1 );
            int middle = ( skip + obj.Count );

            // For the next button
            if ( ( skip + obj.Count ) < total )
            {
                pgN = page + 1;
                skpN = pgN * take;
            }

            // For the previous button
            if ( skip > 0 )
            {
                pgP = ( page > 0 ) ? page - 1 : page;
                skpP = pgP * take;
            }

            return new PagingExtension
            {
                PPage = pgP,
                PSkip = skpP,
                NPage = pgN,
                NSkip = skpN,
                Items = items,
                Start = start,
                Total = total,
                Middle = middle,
                ShowPrev = ( skip > 0 ),
                ShowNext = ( ( skip + obj.Count ) < total )
            };
        }

        public static PagingExtension CreateFromGroupAndItems( object group, object items, int total, int skip, int take, int page )
        {
            List<object> obj = ( ( IEnumerable ) items ).Cast<object>().ToList();

            int skpN = 0, skpP = 0, pgN = 0, pgP = 0;

            int start = ( skip == 0 ) ? ( ( total <= 0 ) ? 0 : 1 ) : ( skip + 1 );
            int middle = ( skip + obj.Count );

            // For the next button
            if ( ( skip + obj.Count ) < total )
            {
                pgN = page + 1;
                skpN = pgN * take;
            }

            // For the previous button
            if ( skip > 0 )
            {
                pgP = ( page > 0 ) ? page - 1 : page;
                skpP = pgP * take;
            }

            return new PagingExtension
            {
                GroupObject = group,
                PPage = pgP,
                PSkip = skpP,
                NPage = pgN,
                NSkip = skpN,
                Items = items,
                Start = start,
                Total = total,
                Middle = middle,
                ShowPrev = ( skip > 0 ),
                ShowNext = ( ( skip + obj.Count ) < total )
            };
        }

    }
}

