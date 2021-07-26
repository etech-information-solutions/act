( function ()
{
    ACT.Loader = {
        Sender: '',

        Image: '<img id="loader" class="div-loader" src="' + imgurl + '/images/loader.gif" alt="" style="margin: 0 5px;" />',

        Html: '<div class="div-loader" style="text-align: center;"><img id="loader" class="apcloud-loader" src="' + imgurl + '/images/loader.gif" alt="" style="margin: 0 5px;" /></div>',

        Show: function ( sender, img )
        {
            if ( sender && sender.length > 0 )
            {
                ACT.Loader.Sender = sender;
                var par = $( sender ).parent();

                //sender.hide();

                if ( img && img === true )
                {
                    par.append( ACT.Loader.Image );
                }
                else if ( img && img === 2 )
                {
                    sender.css( "background", "url('" + imgurl + "/images/loader.gif') no-repeat right center" );
                }
                else
                {
                    par.append( ACT.Loader.Html );
                }
            }

            $( 'html, body' ).css( { 'cursor': 'progress' } );
            $( 'input, textarea, select, a' ).attr( 'disabled', 'disabled' );

            $( 'select.chzn' ).chznreadonly( true );

            // Reset auto log off timer
        },

        Hide: function ()
        {
            $( '.div-loader' ).css( { 'display': 'none' } );

            if ( ACT.Loader.Sender.length )
            {
                ACT.Loader.Sender.css( "background-image", "none" );
                ACT.Loader.Sender.fadeIn( 'slow' );
                ACT.Loader.Sender = '';
            }

            $( '.div-loader' ).remove();

            $( 'select.chzn' ).chznreadonly( false );
            $( 'html, body' ).css( { 'cursor': 'default' } );
            $( 'input, textarea, select, a' ).removeAttr( 'disabled' );
            $( '[data-always-disabled="1"]' ).attr( 'disabled', 'disabled' );
        }
    }
} )();