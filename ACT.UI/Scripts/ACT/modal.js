( function ()
{
    ACT.Modal = {

        MovedObj: [],

        MovedObjSource: [],

        Container: '.modal_act',

        Open: function ( msg, title, show_btns, callback )
        {
            if ( msg )
            {
                $( ACT.Modal.Container ).find( '#modal-body' ).html( msg );
            }

            if ( title )
            {
                $( ACT.Modal.Container ).find( '#modal-title' ).html( title );
            }

            if ( show_btns )
            {
                $( ACT.Modal.Container ).find( '#btns' ).css( "display", "block" );
            }
            else
            {
                $( ACT.Modal.Container ).find( '#btns' ).css( "display", "none" );
            }

            $( ACT.Modal.Container ).fadeIn( 900, function ()
            {
                ACT.UI.DataCallBack( callback );
            } );

            $( '.modalContainer' ).center();
        },

        Close: function ()
        {
            $( ".announcement" ).slideUp( 1200 );

            ACT.UI.DataSettingClient = false;

            $( ACT.Modal.Container ).fadeOut( 500, function ()
            {
                if ( ACT.Modal.MovedObj.length )
                {
                    ACT.Modal.MovedObj.appendTo( ACT.Modal.MovedObjSource );
                }

                $( ACT.Modal.Container ).find( '#modal-body' ).html( '' );
                $( ACT.Modal.Container ).find( '#modal-title' ).html( '' );
                $( ACT.Modal.Container ).removeAttr( "style" );
                $( ACT.Modal.Container ).find( ".modalContent" ).removeAttr( "style" );
            } );
        }
    }
} )();


$( function ()
{
    $( window ).resize( function ()
    {
        $( '.modalContainer' ).center();
    } );
} );
