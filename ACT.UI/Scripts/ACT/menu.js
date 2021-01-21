( function ()
{
    ACT.Menu = {

        ViewPoint: $( ".body" ),

        MenuBar: $( ".ap-tabs > ul" ),

        MenuBarWidth: 0,

        ViewPointWidth: 0,

        Shrinked: false,

        Stretched: false,

        Resize: function ()
        {
            ACT.Menu.MenuBarWidth = 0;

            this.ViewPointWidth = this.ViewPoint.width();

            this.MenuBar.find( "> li" ).each( function ()
            {
                if ( $( this ).hasClass( "last" ) ) return;

                ACT.Menu.MenuBarWidth += $( this ).outerWidth( true );
            } );

            if ( this.ViewPointWidth >= this.MenuBarWidth )
            {
                this.Stretch();

                return;
            }

            this.Shrink();
        },
        Stretch: function ()
        {
            this.MenuBar.find( "> li" ).each( function ()
            {
                $( this ).slideDown( 1200, function ()
                {
                    $( this ).removeClass( "none" );
                } );
            } );

            this.MenuBar.find( "li.mobile-menu" ).remove();

            this.Stretched = true;/**/
        },
        Shrink: function ()
        {
            var avgPieces = parseInt( this.ViewPointWidth / ( this.MenuBarWidth / this.MenuBar.find( "> li" ).length ) );

            var piecesToShow = avgPieces - 1;

            var piecesToHide = [];

            this.MenuBar.find( "> li" ).each( function ( i )
            {
                if ( ( i + 1 ) <= piecesToShow || $( this ).hasClass( "last" ) || $( this ).hasClass( "mobile-menu" ) )
                {
                    $( this ).slideDown( 1200, function ()
                    {
                        $( this ).removeClass( "none" );
                    } );

                    return;
                }

                $( this ).addClass( "none" );

                piecesToHide.push( $( this ) );
            } );

            this.MenuBar.find( "li.mobile-menu" ).remove();

            var mm = "";

            mm += '<li class="mobile-menu">';
            mm += ' <a style="padding: 19px 0; float: right;">';
            mm += '     <i class="fa fa-indent" style="color: #9ca4ab;"></i>';
            mm += ' </a>';
            mm += ' <div id="mobile-menu-items" class="mobile-menu-items">';
            mm += '     <ul class="list">';
            mm += '     </ul>';
            mm += ' </div>';
            mm += '</li>';

            $( mm ).insertBefore( this.MenuBar.find( "li.last" ) );

            for ( var x in piecesToHide )
            {
                this.MenuBar.find( "ul.list" ).append( piecesToHide[x].css( "display", "block" ) );
            }

            this.Shrinked = true;
        }
    }
} )();


$( function ()
{
    ACT.Menu.Resize();

    $( window ).resize( function ()
    {
        ACT.Menu.Resize();
    } );
} );