( function ()
{
    ACT.UI = {

        URL: '',

        PageSkip: 0,

        PageNumber: 0,

        PageViewId: 0,

        PageSearch: '',

        PageSort: 'DESC',

        PageSortBy: 'CreatedOn',

        PageLength: 50,

        PageBroadcast: 0,

        PageActionDate: 0,

        PageSearchTimer: [],

        PageStayAliveTimer: [],

        SelectedItems: [],

        PageErrorOcurred: false,

        PageViewIdProcessed: false,

        DocumentTypes: ["pdf", "doc", "docx", "gif", "jpg", "jpeg", "png", "bmp", "tif", "tiff", "csv", "xls", "xlsx"],

        SAIDRegex: /^(((\d{2}((0[13578]|1[02])(0[1-9]|[12]\d|3[01])|(0[13456789]|1[012])(0[1-9]|[12]\d|30)|02(0[1-9]|1\d|2[0-8])))|([02468][048]|[13579][26])0229))(( |-)(\d{4})( |-)([01]8((( |-)\d{1})|\d{1}))|(\d{4}[01]8\d{1}))/g,

        Start: function ()
        {
            this.DataGraphs( $( '*[data-graph="1"]' ) );

            var dt = ( $( '#tab-data>div:visible' ).length ) ? $( '#tab-data>div:visible' ) : ( $( '#collapse>div:visible' ).length ) ? $( '#collapse>div:visible' ) : ( $( "#list" ).length ) ? $( "#list" ) : $( ".graphs" ).length ? $( ".graphs" ) : $( '#item-list' );

            if ( dt.find( "#collapse" ).length )
            {
                dt = dt.find( "#collapse>div:visible" );
            }

            this.DataTablesOverride( dt );

            this.DataTablesDateRange( $( "#tab-data > div" ) );
            this.DataPartialImages( $( '*[data-partial-images="1"]' ) );

            this.DataHover( $( '*[data-hover="1"]' ) );

            this.DataAPTab( $( '*[data-ap-tab="1"]' ) );
            this.DataRefresh( $( '*[data-refresh="1"]' ) );


            this.DataModal( $( '*[data-modal="1"]' ) );
            this.DataAjaxForm( $( '*[data-ajax-form="1"]' ) );
            this.DataShowHide( $( '*[data-show-hide="1"]' ) );
            this.DataStickyOne( $( '*[data-sticky-one="1"]' ) );
            this.DataAddOneMore( $( '*[data-add-one-more="1"]' ) );
            this.DataDelOneMore( $( '*[data-del-one-more="1"]' ) );
            this.DataDeleteImage( $( '*[data-delete-image="1"]' ) );
            this.DataUploadImage( $( '*[data-upload-image="1"]' ) );
            this.DataDeleteDocument( $( '*[data-delete-document="1"]' ) );

            // Table CRUD Operations
            this.DataEdit( $( '*[data-edit="1"]' ) );
            this.DataRole( $( '*[data-Role="1"]' ) );
            this.DataCancel( $( '*[data-cancel="1"]' ) );
            this.DataDelete( $( '*[data-delete="1"]' ) );
            this.DataDetails( $( '*[data-details="1"]' ) );
            this.DataAuditLog( $( '*[data-audit-log="1"]' ) );
            this.DataFormSubit( $( '*[data-form-submit="1"]' ) );
            this.DataCancelItem( $( '*[data-cancel-item="1"]' ) );
            this.DataApproveDeclinePSP( $( '*[data-approve-decline-psp="1"]' ) );


            // Table Quick Links Operations
            this.DataStep( $( '*[data-step="1"]' ) );
            this.DataCollapse( $( '*[data-collapse="1"]' ) );
            this.DataCheckOptions( $( '*[data-check-options="1"]' ) );

            // PR Create / Edit
            this.DataBank( $( '*[data-bank="1"]' ) );

            // Finance
            this.DataShowSelected( $( '*[data-show-selected="1"]' ) );

            // Length Validation
            this.DataValMax( $( '*[data-val-length-max]' ) );

            // Bank Details Validation
            this.DataBankValidation( $( '*[data-bank-val]' ) );


            // Money
            this.DataMoney( $( '*[data-money="1"]' ) );


            // Sign Up
            this.DataDOB( $( '*[data-dob="1"]' ) );
            this.DataServiceType( $( '*[data-service-type="1"]' ) );

            // Dashboard / Graphs
            this.DataGSSite( $( '*[data-gs-site="1"]' ) );
            this.DataGSRegion( $( '*[data-gs-region="1"]' ) );

            this.DataGSData( $( '[data-gs-data="1"]' ) );
            this.DataGSearch( $( '*[data-g-search="1"]' ) ); // Search button on Dashboard

            if ( window.location.search !== "" && !$( "tr.edit" ).length && $( ".dataTable" ).length && !ACT.UI.PageViewIdProcessed )
            {
                var viewid = false,
                    open = "details";

                var search = window.location.search.replace( "?", "" ).split( "&" );
                for ( var i = 0; i < search.length; i++ )
                {
                    var xxs = search[i].split( "=" );

                    if ( xxs[0].toLowerCase() === "prid" && ( $( '#tr-' + xxs[1] + '-item [data-edit="1"]' ).length || $( '#tr-' + xxs[1] + '-item [data-details="1"]' ) ) )
                    {
                        viewid = xxs[1];
                    }
                    if ( xxs[0].toLowerCase() === "open" )
                    {
                        open = xxs[1];
                    }
                }

                if ( viewid && open )
                {
                    $( '#tr-' + viewid + '-item [data-' + open + '="1"]' ).click();

                    ACT.UI.PageViewIdProcessed = true;
                }
            }

            this.AutoLogOff( lgt );
            this.DataStayAlive();


            // Broadcast
            this.DataGetBroadcast();

            $( 'input[readonly=""], select[readonly=""], textarea[readonly=""]' ).removeAttr( 'readonly' );
            $( 'input[disabled=""], select[disabled=""], textarea[disabled=""]' ).removeAttr( 'disabled' );
        },

        DataSideBar: function ( sender )
        {

            sender.each( function ()
            {
                var i = $( this );

                var clicked = i.attr( "data-clicked" );
                var target = $( i.attr( "data-target" ) );
                var content = $( i.attr( "data-content" ) );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        i.attr( "data-clicked", "1" );

                        var width = "190px";
                        var state = i.attr( "data-state" );

                        if ( state === "open" )
                        {
                            width = "60px";
                            target.addClass( "close" );
                            i.find( ".fa" ).removeClass( "fa-outdent" ).addClass( "fa-indent" );
                            i.attr( { "data-state": "close", "title": "Open side menu?" } );
                        }
                        else
                        {
                            target.removeClass( "close" );
                            i.find( ".fa" ).removeClass( "fa-indent" ).addClass( "fa-outdent" );
                            i.attr( { "data-state": "open", "title": "Hide side menu?" } );
                        }

                        target.animate( {
                            "width": width
                        }, 1000, function ()
                        {

                        } );

                        content.animate( {
                            "marginLeft": width
                        }, 1000, function ()
                        {

                        } );
                    } );

                if ( clicked === "0" )
                {
                    i.click();
                }
            } );
        },

        AutoLogOff: function ( s, go )
        {
            if ( s === "-1" || s === "-1m0s" || s === "0h0m0s" ) return;

            var tick = $( '*[data-timer-tick="1"]' );
            var pap = tick.parent().parent();
            var cont = pap.find( 'a[data-continue="1"]' );

            // Reset timer (if any) 
            tick.timer( "remove" );

            tick.timer( {
                countdown: true,
                duration: s,
                callback: function ()
                {
                    if ( !go )
                    {
                        tick.timer( "remove" );

                        if ( ACT.Modal.MovedObj.length )
                        {
                            ACT.Modal.MovedObj.appendTo( ACT.Modal.MovedObjSource );
                        }

                        $( ACT.Modal.Container ).find( '#modal-body' ).html( '' );
                        $( ACT.Modal.Container ).find( '#modal-title' ).html( '' );

                        var title = "Auto Logoff";
                        var data = pap;

                        ACT.Modal.MovedObj = data.children();
                        ACT.Modal.MovedObjSource = data;

                        data.children().appendTo( $( ACT.Modal.Container ).find( '#modal-body' ) );

                        ACT.Modal.Open( null, title );

                        setTimeout( function ()
                        {
                            ACT.UI.AutoLogOff( 3 + "m" + 0 + "s", true );
                        }, "100" );
                    }
                    else
                    {
                        // Logout...
                        //window.location = "/Account/LogOff?r=alo";

                        $.get( "/Account/PartialLogOff", {}, function ( data, s, xhr )
                        {
                            ACT.Modal.Close();

                            var date = new Date( null );
                            date.setSeconds( cas ); // specify value for SECONDS here
                            var result = date.toISOString().substr( 11, 8 );

                            var title = "You've been logged out";
                            var msg = "<p>YOU HAVE BEEN LOGGED OUT OF THE SYSTEM DUE TO NO ACTIVITY FOR THE PAST " + result + ".</p>";
                            msg += "<p>PLEASE CLOSE YOUR BROWSER, RE-OPEN THE APPLICATION AND LOG IN, SHOULD YOU NEED TO CONTINUE WORKING</p>";
                            msg += "<p>THANK YOU</p>";

                            setTimeout( function ()
                            {
                                ACT.Modal.Open( msg, title );
                            }, "1000" );
                        } );
                    }
                }
            } );

            cont
                .unbind( "click" )
                .bind( "click", function ()
                {
                    // Reset timer
                    tick.timer( "remove" );

                    setTimeout( function ()
                    {
                        ACT.UI.AutoLogOff( lgt );
                    }, "800" );

                    ACT.UI.DataStayAlive();

                    ACT.Modal.Close();
                } );
        },

        DataRenew: function ( s )
        {
            if ( s === "-1" ) return;

            clearTimeout( ACT.UI.PageRenewTimer );

            ACT.UI.PageRenewTimer = setTimeout( function ()
            {
                $.get( "/Account/Renew", {}, function ( data, s, xhr )
                {
                } );
            }, s );
        },

        DataStayAlive: function ()
        {
            clearTimeout( ACT.UI.PageStayAliveTimer );

            ACT.UI.PageStayAliveTimer = setTimeout( function ()
            {
                $.get( siteurl + "/StayAlive", {}, function ( data, s, xhr )
                {
                    ACT.UI.DataStayAlive();
                } );
            }, "300000" );
        },

        DataAPTab: function ( sender )
        {
            var hash = window.location.hash;

            sender.each( function ( c )
            {
                var i = $( this );

                var target = $( i.attr( "data-target" ) );
                var reload = $( i.attr( "data-reload" ) );
                var holder = $( i.attr( "data-tab-holder" ) );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        var rendered = parseInt( i.attr( "data-rendered" ) );

                        if ( ( target.is( ":visible" ) && rendered ) || ( i.hasClass( "not-allowed" ) ) ) return;

                        window.location.hash = i.attr( "data-target" );
                        sender.removeClass( "current" );

                        holder.find( ">div.current" ).css( "display", "none" );

                        i.addClass( "current" );

                        if ( target.find( "table.fixedHeader-floating" ).length > 0 )
                        {
                            var table = target.find( "table.fixed-header" );
                            var header = target.find( "table.fixedHeader-floating" );

                            ACT.UI.ShowFixedHeader( header, table );
                        }

                        target.fadeIn( 1200, function ()
                        {
                            $( this ).addClass( "current" );
                        } );

                        ACT.UI.DataPartialLoad( i, sender );
                    } );

                if ( ( c === 0 && hash === '' ) || ( i.hasClass( "current" ) && hash === i.attr( "data-target" ) ) || ( !i.hasClass( "current" ) && hash === i.attr( "data-target" ) ) )
                {
                    i.click();
                }
            } );
        },

        DataCollapse: function ( params )
        {
            var hash = window.location.hash;

            params.each( function ( c )
            {
                // Get current instance
                var i = $( this );

                var holder = $( i.attr( "data-tab-holder" ) );

                // Get target to be updated with server response
                var target = $( i.attr( 'data-target' ) );
                target = ( target.length <= 0 ) ? $( i.attr( "target" ) ) : target;

                // Unbind any click events
                i.unbind( 'click' );
                i.click( function ()
                {
                    var rendered = parseInt( i.attr( "data-rendered" ) );

                    //if ( target.is( ":visible" ) )
                    //{
                    //    target.slideUp( 900, function ()
                    //    {
                    //        i
                    //        .removeClass( "open" )
                    //        .addClass( "closed current" );
                    //    } );

                    //    return;
                    //}

                    if ( ( target.is( ":visible" ) && rendered ) || ( i.hasClass( "not-allowed" ) ) ) return;

                    window.location.hash = i.attr( "data-target" );

                    params
                        .addClass( "closed" )
                        .removeClass( "current open" );

                    holder.find( ".current" ).css( "display", "none" );

                    i
                        .removeClass( "closed" )
                        .addClass( "open current" );

                    target.fadeIn( 1200, function ()
                    {
                        $( this ).addClass( "current" );
                    } );

                    ACT.UI.DataPartialLoad( i, params );
                } );

                if ( ( c === 0 && hash === '' ) || ( i.hasClass( "current" ) && hash === i.attr( "data-target" ) ) || ( !i.hasClass( "current" ) && hash === i.attr( "data-target" ) ) )
                {
                    i.click();
                }
            } );
        },

        DataStep: function ( sender )
        {
            sender.each( function ()
            {
                var me = $( this );

                var target = $( me.attr( "data-target" ) );
                var rendered = me.attr( "data-rendered" );
                var group = $( '[data-target="' + me.attr( "data-target" ) + '"]' );

                me
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        if ( target.is( ":visible" ) ) return;

                        var cntr = [];
                        var valid = true;
                        var direction = "center-left";
                        var err = "<div class='message-error'>";

                        if ( valid && $( "#select-originator" ).is( ":visible" ) && $( "#select-originator select#Id" ).val() === "" )
                        {
                            valid = false;
                            cntr = $( "#select-originator select#Id" ).parent().find( "div.chzn" );
                            err += "Start off by selecting the originator who owns the PR (s) to change ownership...";
                        }

                        if ( valid && me.attr( "data-target" ) === "#change-originator" && $( "#select-new-originator select#Id" ).val() === "" )
                        {
                            if ( !$( "#select-new-originator" ).is( ":visible" ) )
                            {
                                show( sender, $( '[data-step="1"][data-target="#select-new-originator"]' ), $( "#select-new-originator" ) );
                            }

                            valid = false;
                            cntr = $( "#select-new-originator select#Id" ).parent().find( "div.chzn" );
                            err += "Next up, select a new originator who'll own the PR (s)...";
                        }

                        if ( valid && me.attr( "data-target" ) === "#change-originator" && $( "#select-new-originator select#Id" ).val() === $( "#select-originator select#Id" ).val() )
                        {
                            if ( !$( "#select-new-originator" ).is( ":visible" ) )
                            {
                                show( sender, $( '[data-step="1"][data-target="#select-new-originator"]' ), $( "#select-new-originator" ) );
                            }

                            valid = false;
                            cntr = $( "#select-new-originator select#Id" ).parent().find( "div.chzn" );
                            err += "You just selected the same originator from Step 1, please select a different originator and hit Next...";
                        }

                        if ( valid && $( "#select-pr" ).length && $( "#select-pr" ).is( ":visible" ) && !$( '#select-pr table input[type="checkbox"]:checked' ).length )
                        {
                            valid = false;
                            direction = "center-left";
                            cntr = $( '#select-pr table input[type="checkbox"]:first' );
                            err += "Start off by selecting Payment Requisitions you would like to authorise in the table below. Tip: Check this one to <b>Select All</b>";
                        }

                        if ( !valid )
                        {
                            err += "</div>";

                            ACT.Sticky.StickyOne.addClass( "error" );
                            ACT.Sticky.StickyOne.css( { "display": "none" } );

                            ACT.Sticky.Show( cntr, "We can't go next yet!", err, [], direction );
                            $( 'html, body' ).animate( { scrollTop: cntr.offset().top - 150 }, 'slow', function () { cntr.focus(); } );

                            return valid;
                        }
                        else
                        {
                            Show( sender, me, target );
                        }

                        return false;
                    } );

                if ( $( "#select-pr" ).length )
                {
                    ReIndex( $( "#select-pr" ) );
                    RestoreSelectedItems( $( "#select-pr" ), $( "#authorise-pr" ) );
                }

                if ( $( '#select-pr input[data-check-all="1"]' ).length )
                {
                    var checkAll = $( '#select-pr input[data-check-all="1"]' );

                    kids = $( checkAll.attr( "data-kids" ) );

                    if ( ACT.UI.SelectedItems.length )
                    {
                        checkAll.prop( "checked", true ).attr( "checked", "checked" );
                    }

                    checkAll
                        .unbind( "change" )
                        .bind( "change", function ()
                        {
                            if ( $( this ).is( ":checked" ) )
                            {
                                kids.prop( "checked", true ).attr( "checked", "checked" );
                            }
                            else
                            {
                                kids.prop( "checked", false ).removeAttr( "checked" );
                            }

                            $( '#select-pr table tbody input[type="checkbox"]' ).change();
                        } );
                }

                target.find( '[data-select-originator="1"]' ).each( function ()
                {
                    var s = $( this );

                    var od = $( s.attr( "data-od" ) );
                    var pr = $( s.attr( "data-pr-list" ) );

                    var odurl = s.attr( "data-od-url" );
                    var prurl = s.attr( "data-pr-url" );

                    s
                        .unbind( "change" )
                        .bind( "change", function ()
                        {
                            ACT.Loader.Show( s, true );

                            odurl = siteurl + odurl;
                            od.load( odurl, { id: $( this ).val() }, function ()
                            {
                                ACT.Init.Start( true );
                            } );

                            prurl = siteurl + prurl;
                            pr.load( prurl, { id: $( this ).val() }, function ()
                            {
                                ACT.Init.Start( true );
                                var checksIn = pr.find( "form table tbody" );

                                ReIndex( checksIn );

                                pr.find( 'form table thead input[type="checkbox"]' )
                                    .unbind( "change" )
                                    .bind( "change", function ()
                                    {
                                        if ( $( this ).is( ":checked" ) )
                                        {
                                            checksIn.find( 'input[type="checkbox"]' ).prop( "checked", true ).attr( "checked", "checked" ).change();
                                        }
                                        else
                                        {
                                            checksIn.find( 'input[type="checkbox"]' ).prop( "checked", false ).removeAttr( "checked" );
                                        }
                                    } );
                            } );
                        } );
                } );

                target.find( '[data-select-new-originator="1"]' )
                    .unbind( "change" )
                    .bind( "change", function ()
                    {
                        $( "#NewOriginatorId" ).val( $( this ).val() );
                        $( "#new-originator-name" ).text( $( this ).children( "option" ).filter( ":selected" ).text() );
                    } );

                function ReIndex( target )
                {
                    target.find( 'table tbody input[type="checkbox"]' )
                        .unbind( "change" )
                        .bind( "change", function ()
                        {
                            ACT.UI.DataIndex( target.find( 'input[type="checkbox"]:checked' ) );

                            var ind = SelectedItemExist( $( this ).val() );

                            if ( !$( this ).is( ":checked" ) && ind >= 0 )
                            {
                                ACT.UI.SelectedItems.splice( ind, 1 );
                            }

                            if ( $( "#select-pr" ).length && $( '#select-pr table input[type="checkbox"]:checked' ).length )
                            {
                                $( '#select-pr table input[type="checkbox"]:checked' ).each( function ()
                                {
                                    if ( typeof $( this ).attr( "Data-id" ) === 'undefined' ) return;

                                    var id = $( this ).val();
                                    var number = $( this ).parent().parent().find( "#pr-number-span" ).text() || '';

                                    if ( SelectedItemExist( id ) < 0 )
                                    {
                                        ACT.UI.SelectedItems.push( { "Id": id, "Number": number } );
                                    }
                                } );
                            }

                            if ( target.find( "#sel-pr-count" ).length )
                            {
                                target.find( "#sel-pr-count" ).text( ACT.UI.SelectedItems.length + " Item (s) Selected" );
                            }
                        } );

                    if ( target.find( "#sel-pr-count" ).length )
                    {
                        target.find( "#sel-pr-count" ).text( ACT.UI.SelectedItems.length + " Item (s) Selected" );
                    }
                }

                function SelectedItemExist( id )
                {
                    for ( var i = 0; i < ACT.UI.SelectedItems.length; i++ )
                    {
                        if ( ACT.UI.SelectedItems[i].Id === id ) return i;
                    }

                    return -1;
                }

                function Show( sender, me, target )
                {
                    sender.removeClass( "active" );//pr-number-span

                    $( ".step" ).css( "display", "none" );
                    target.css( "display", "block" );
                    me.add( '[data-target="' + me.attr( "data-target" ) + '"]' ).addClass( "active" ).attr( "data-rendered", "1" );

                    target.find( "#pr-preview" ).html( '' );

                    RestoreSelectedItems( $( "#select-pr" ), $( "#authorise-pr" ) );

                    if ( $( "#select-pr" ).length && me.attr( "data-number" ) === "2" && me.attr( "data-loaded" ) === "0" )
                    {
                        ACT.Loader.Show( target.find( "#details #sel-pr-loader" ), false );

                        $.get( siteurl + "/CompleteAuthorisation", {}, function ( data, status, req )
                        {
                            target.find( "#details" ).html( data );

                            target.find( "#pr-preview" ).show( 1200 );

                            ACT.Loader.Hide();
                            me.attr( "data-loaded", "1" );

                            ACT.Init.PluginInit( target );

                            ACT.UI.DataValMax( $( '*[data-val-length-max]' ) );
                            ACT.UI.DataAjaxForm( $( '*[data-ajax-form="1"]' ) );
                            ACT.UI.DataCheckOTP( $( '*[data-check-otp="1"]' ) );
                            ACT.UI.DataResendOTP( $( '*[data-resend-otp="1"]' ) );
                        } );
                    }

                    target.animate( { scrollTop: target.offset().top - 50 }, 'slow', function () { } );
                }

                function RestoreSelectedItems( target, preview )
                {
                    if ( ACT.UI.SelectedItems.length )
                    {
                        for ( var i = 0; i < ACT.UI.SelectedItems.length; i++ )
                        {
                            if ( typeof ACT.UI.SelectedItems[i].Id === 'undefined' ) return;

                            var inp = '<input name="SelectedPRList[' + i + ']" type="hidden" value="' + ACT.UI.SelectedItems[i].Id + '" />';
                            var s = '<span style="display: inline-block; border: 1px dashed #ddd; border-radius: 2px; padding: 4px; margin: 0 4px; 4px 0;">' + ACT.UI.SelectedItems[i].Number.trim() + '</span>';

                            preview.find( "#pr-preview" ).append( s );
                            preview.find( "#pr-preview" ).append( inp );

                            if ( target.find( 'input[type="checkbox"][data-id="' + ACT.UI.SelectedItems[i].Id + '"]' ).length )
                            {
                                target.find( 'input[type="checkbox"][data-id="' + ACT.UI.SelectedItems[i].Id + '"]' )
                                    .prop( "checked", true )
                                    .attr( "checked", "checked" );
                            }
                        }
                    }
                }
            } );

            sender.first().click();
        },

        DataHover: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var oupa = i.parent();

                if ( oupa.hasClass( "disabled" ) ) return;

                var src = i.attr( "data-src" );
                var originalSrc = i.attr( "src" );

                i
                    .unbind( "moouseout" )
                    .unbind( "moouseover" )

                    .bind( "mouseout", function ()
                    {
                        i.attr( "src", originalSrc );
                    } )
                    .bind( "mouseover", function ()
                    {
                        i.attr( "src", src );
                    } );
            } );
        },

        DataRefresh: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var target = ( $( '.ap-tabs li a[data-target="' + i.attr( "data-target" ) + '"]' ).length ) ? $( '.ap-tabs li a[data-target="' + i.attr( "data-target" ) + '"]' ) :
                    ( $( '.collapse strong[data-target="' + i.attr( "data-target" ) + '"]' ).length ) ? $( '.collapse strong[data-target="' + i.attr( "data-target" ) + '"]' ) :
                        $( i.attr( "data-target" ) );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        ACT.UI.ClearCustomSearch( i.attr( "data-target" ).replace( "#", "" ) );

                        if ( target.hasClass( "graphs" ) )
                        {
                            target.attr( { "data-loaded": 0, "data-loading": 0 } );

                            ACT.UI.DataGraph( target );

                            return;
                        }

                        var atarget = $( target.attr( "data-target" ) );

                        atarget.html( "" );
                        target.attr( "data-rendered", 0 );
                        target.click();
                        $( ".tipsy" ).remove();

                        ACT.UI.SelectedItems = [];
                    } );
            } );
        },

        DataGetQueryString: function ( key )
        {
            if ( window.location.search === "" )
            {
                return "";
            }

            var search = window.location.search.replace( "?", "" ).split( "&" );

            for ( var ix = 0; ix < search.length; ix++ )
            {
                var xxs = search[ix].split( "=" );

                if ( xxs[0].toLowerCase() === key.toLowerCase() )
                {
                    return xxs[1];
                }
            }

            return "";
        },

        DataPartialLoad: function ( sender, group )
        {
            var count = 1;

            sender.each( function ()
            {
                var i = $( this );

                var url = i.attr( "data-load-url" );
                var target = $( i.attr( "data-target" ) );

                var rendered = parseInt( i.attr( "data-rendered" ) );

                if ( rendered === 1 || count > 1 )
                {
                    if ( target.find( '[data-collapse="1"]' ).length )
                    {
                        //target.find('[data-collapse="1"]').first().click();
                    }

                    return true;
                }

                url = ( url.indexOf( "/" ) !== -1 ) ? url : siteurl + url;

                var load = '<div class="partial-loading"><img alt="" src="' + imgurl + '/images/loader.gif" /></div>';
                var spinner = '<div class="spinner"><img alt="" src="' + imgurl + '/images/spinner.gif" /></div>';

                var results = '<div class="partial-results"></div>';

                target.append( load );
                target.append( results );

                var by = null;

                if ( window.location.search !== "" )
                {
                    var search = window.location.search.replace( "?", "" ).split( "&" );
                    for ( var ix = 0; ix < search.length; ix++ )
                    {
                        var xxs = search[ix].split( "=" );

                        if ( xxs[0].toLowerCase() === "skip" )
                        {
                            ACT.UI.PageSkip = xxs[1];
                        }
                        if ( xxs[0].toLowerCase() === "prid" )
                        {
                            ACT.UI.PageViewId = xxs[1];
                        }
                        if ( xxs[0].toLowerCase() === "budgetyear" )
                        {
                            by = xxs[1];
                            ACT.UI.PageBudgetYear = xxs[1];
                        }
                    }
                }

                target.find( ".partial-loading" ).stop().animate( {
                    "opacity": "1",
                    "width": "100%",
                    "padding": "20px 0",
                    "filter": "alpha(opacity=100)"
                }, 1200, function ()
                {
                    i.parent().prepend( spinner );

                    group.addClass( "not-allowed" );
                    target.find( ".partial-results" ).stop().load( url, { skip: ACT.UI.PageSkip, PRId: ACT.UI.PageViewId, BudgetYear: by }, function ( r, s, xhr )
                    {
                        if ( s === "error" )
                        {
                            ACT.Modal.Open( xhr.responseText, xhr.statusText, false, ACT.Init.Start() );

                            return;
                        }

                        var table = $( this ).find( "table.datatable-numberpaging" );

                        // Tables Excused...
                        if ( table.find( "tbody tr td" ).length > 1 )
                        {
                            var sort = table.hasClass( "sort" );

                            table.dataTable( {
                                bPaginate: false,
                                bSort: false,
                                iDisplayLength: 50,
                                //"fixedHeader": {
                                //    header: table.hasClass( "fixed-table" )
                                //},
                                "fnDrawCallback": function ()
                                {
                                    ACT.UI.Start();
                                }
                            } );
                        }

                        $( this ).stop().animate( {
                            "opacity": "1",
                            "width": "100%",
                            "filter": "alpha(opacity=100)"
                        }, 1200, function ()
                        {

                        } );

                        target.find( ".partial-loading" ).remove();

                        i.attr( "data-rendered", 1 );
                        i.parent().find( ".spinner" ).stop().fadeOut( 1000, function () { $( this ).remove(); } );

                        ACT.Init.Start( true );
                        ACT.UI.DataTablesOverride( target );
                        ACT.UI.DataPRSum( $( '*[data-pr-sum="1"]' ) );

                        group.removeClass( "not-allowed" );
                    } );
                } );

                count++;
            } );
        },

        DataAjaxForm: function ( sender )
        {
            var empty = "#empty-div";

            var islink = false;

            sender.each( function ()
            {
                var i = $( this );

                var t = $( i.attr( "data-target" ) );

                var justhide = i.attr( "data-just-hide-buttons" ) === "1";

                if ( justhide && typeof i.attr( "data-do-update" ) === "undefined" )
                {
                    t = empty;
                }

                var options =
                {
                    target: t, // target element to be updated with server response
                    beforeSubmit: function ( formData, jqForm, options )
                    {
                        islink = i.find( "#islink" ).length > 0 && i.find( "#islink" ).val() === "True";

                        if ( i.hasClass( "custom-validate" ) && !ACT.UI.DataValidateForm( jqForm ) )
                        {
                            return false;
                        }

                        if ( islink && i.find( "#islink" ).attr( "name" ) !== "decline" )
                        {
                            options.target = empty;
                        }
                        else
                        {
                            options.target = t;
                        }

                        ACT.Loader.Show( islink ? i.find( '#sdoc-btn' ) : i.find( '#save-btn' ), true );

                        islink = i.find( "#islink" ).attr( "name" ) === "decline" ? false : islink;

                    },  // pre-submit callback 
                    success: function ( data, status, xhr, x )
                    {
                        if ( islink )
                        {
                            window.location = "/Products/Index?gotoaction=AddProduct#products";
                        }

                        var td = $( "#editing-td" );

                        if ( justhide && td.length > 0 && $( t ).find( ".message-success" ).length )
                        {
                            var tr = $( td.attr( "target" ) );

                            td.parent().remove();

                            tr.addClass( "green-bg" );

                            tr.find( 'a[data-edit="1"]' ).each( function ()
                            {
                                $( this ).hide( 900 ).remove();
                            } );
                        }
                        else if ( justhide && td.length > 0 && ( $( t ).find( ".message-error" ).length || $( t ).find( ".message-warn" ).length ) )
                        {
                            td.html( data );
                        }
                        else if ( justhide && typeof i.attr( "data-do-update" ) !== "undefined" )
                        {
                            var tr1 = $( td.attr( "target" ) );

                            td.parent().remove();
                            tr1.addClass( "green-bg" );
                        }

                        if ( justhide && typeof i.attr( "data-do-update" ) === "undefined" )
                        {
                            $( t ).html( "" );
                        }

                        ACT.UI.SelectedItems = [];

                        ACT.Init.PluginLoaded = false;
                        ACT.Init.Start();

                    },  // post-submit callback
                    error: function ( data )
                    {
                        var cntr = islink ? i.find( '#sdoc-btn' ) : i.find( '#save-btn' );

                        ACT.Sticky.StickyOne.addClass( "error" );
                        ACT.Sticky.StickyOne.css( { "display": "none" } );

                        ACT.Sticky.Show( cntr, "Oops! Something went wrong", data, ACT.Init.Start(), "bottom-left" );
                    }
                };

                i.ajaxForm( options );
            } );
        },

        DataSwitchTabs: function ( from, to )
        {
            var xpr = $( 'li a[data-target="' + from + '"]' );
            var xsd = $( 'li a[data-target="' + to + '"]' );

            var xtarget = $( xsd.attr( "data-target" ) );

            xpr.removeClass( "current" );
            $( from ).removeClass( "current" ).css( "display", "none" );

            xtarget.fadeIn( 1200, function ()
            {
                $( this ).addClass( "current" );
                xsd.attr( "data-rendered", 1 ).addClass( "current" );
            } );

            window.location.hash = xsd.attr( "data-target" );
        },

        DataDeleteImage: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var pid = i.attr( "data-pid" );
                var url = i.attr( "data-url" );
                var vid = i.attr( "data-vid" );

                var p = i.closest( "p" );
                var target = $( i.attr( 'data-target' ) + ":visible" );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        $( ".tipsy" ).remove();

                        if ( pid === "-1" )
                        {
                            var d = p.find( 'textarea' );
                            var f = p.find( 'input[type="file"]' );
                            var img = p.find( 'img.preview' );

                            if ( f.val() !== "" )
                            {
                                f.val( "" );
                                d.val( "" );
                                img.attr( "src", img.attr( "data-original-src" ) );
                            }

                            return;
                        }

                        var msg = "";

                        var title = "Delete this image?";
                        msg += "<p style='margin: 0;'>Are you sure you would like to delete this image?<br />The image will indeed be gone for good!</p>";
                        msg += '<div style="border-bottom: 1px dashed #ccc; margin-bottom: 10px; height: 0;" class="clear">&nbsp;</div>';
                        msg += "<input id='del-no' type='button' value='No!' style='background: #000000;' /><span style='padding: 0 6px;'>/</span><input id='del-yes' type='button' value='YES' />";

                        ACT.Sticky.StickyOne.css( { "display": "none" } );
                        ACT.Sticky.Show( i, title, msg, [], "center-right" );

                        var m = ACT.Sticky.StickyOne.find( ".sticky-data" );

                        var no = m.find( "#del-no" );
                        var yes = m.find( "#del-yes" );

                        no
                            .unbind( "click" )
                            .bind( "click", function ()
                            {
                                ACT.Sticky.Hide();
                            } );

                        yes
                            .unbind( "click" )
                            .bind( "click", function ()
                            {
                                ACT.Loader.Show( i, true );

                                target.load( url, { pid: pid, vid: vid }, function ()
                                {
                                    ACT.Sticky.Hide();
                                    ACT.Loader.Hide();
                                    ACT.Init.Start( true );
                                } );
                            } );

                        return false;
                    } );
            } );
        },

        DataDeleteDocument: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var id = i.attr( "data-id" );
                var target = $( i.attr( "data-target" ) );

                var msg = "";
                var title = "Delete Document?";

                msg += "<p>Are you sure you would like to delete this document?<br />This action is permanent.</p>";
                msg += "<p style='margin: 0;'>";
                msg += "    <input id='delete-" + id + "-doc' type='button' value='Delete' />";
                msg += "    <a onclick='ACT.Sticky.Hide()' style='padding-left: 4px;'>Cancel</a>";
                msg += "</p>";

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        ACT.Sticky.Show( i.find( "img" ), title, msg, [], "center-left" );

                        var btn = $( "#delete-" + id + "-doc" );

                        btn
                            .unbind( "click" )
                            .bind( "click", function ()
                            {
                                ACT.Loader.Show( btn, true );

                                $( "#empty-div" ).load( siteurl + "/DeleteDocument", { id: id }, function ()
                                {
                                    ACT.Loader.Hide();
                                    ACT.Sticky.Hide();

                                    target.hide( 900 ).remove();
                                } );
                            } );
                    } );
            } );
        },

        DataUploadImage: function ( sender )
        {
            var img = $( ".image-preview:visible" );

            sender.each( function ()
            {
                var i = $( this );

                i
                    .unbind( "change" )
                    .bind( "change", function ()
                    {
                        if ( this.files && this.files[0] )
                        {
                            var reader = new FileReader();

                            reader.onload = function ( e )
                            {
                                img.attr( "src", e.target.result );
                            };

                            // Read in the image file as a data URL.
                            reader.readAsDataURL( this.files[0] );
                        }
                    } );
            } );
        },

        DataShowHide: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var show = $( i.attr( "data-show" ) );
                var hide = $( i.attr( "data-hide" ) );

                var req = i.attr( "data-show-required" );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        hide.css( "display", "none" );


                        show.fadeIn( 1200, function ()
                        {
                            ACT.UI.DataHighlightFields( show );
                        } );

                        if ( typeof req !== 'undefined' && req === "1" )
                        {
                            hide.find( "input[id], textarea[id], select[id]" ).removeAttr( "required" );
                            show.find( "input[id], textarea[id], select[id]" ).attr( "required", "required" );
                        }
                    } );
            } );
        },

        DataAddOneMore: function ( sender )
        {
            // Add one more image kiara@sheenah1
            sender.each( function ()
            {
                var i = $( this );

                var target = $( i.attr( 'data-target' ) );
                var autoIncrement = i.attr( 'data-auto-increment' );

                i.unbind( 'click' );
                i.click( function ()
                {
                    var total = 1;
                    var html = "";
                    var clone = [];

                    if ( target.is( "tr" ) )
                    {
                        var papa = target.parent();

                        clone = papa.find( ".add-more-row:first" ).clone();
                        var inputs = clone.find( '.input, input[type="hidden"], input[type="text"], input[type="password"], select, textarea' );

                        inputs.each( function ()
                        {
                            $( this ).val( "" );

                            if ( $( this ).is( "select" ) )
                            {
                                $( this ).find( "option" ).removeAttr( "selected" );
                            }
                        } );

                        // Extra clean up
                        clone.find( '[data-warn="1"]' ).remove();
                        clone.find( '[data-ob="1"]' ).text( '-/-' );
                        clone.find( '[data-rb="1"]' ).text( '-/-' );
                        clone.find( '[data-pr-amount="1"]' ).css( 'width', '88%' );

                        total = papa.find( ".add-more-row" ).length;

                        html = clone.html().replace( /\[0]/g, "[" + total + "]" ).replace( /\-0-/g, "-" + total + "-" );
                        clone.html( html );

                        clone.find( '.slick-counter' ).html( '' );
                        clone.find( '.input, input[type="hidden"], input[type="text"], input[type="password"], select, textarea' ).val( "" );

                        ACT.UI.RecreatePlugins( clone );

                        clone.insertAfter( papa.find( ".add-more-row:last" ) );

                        clone.find( 'a[data-add-one-more="1"]' ).fadeOut( 1200, function ()
                        {
                            $( this ).remove();
                        } );

                        papa.find( 'a[data-add-one-more="1"]' ).fadeOut( 1200, function ()
                        {
                            var f = clone.find( "td:first" ).children( ":first" );
                            $( this ).insertBefore( f ).fadeIn( 1200 );
                        } );
                    }
                    else
                    {
                        // Get clone instance as a jquery object
                        clone = target.children().first().clone();

                        // Check if this clone needs auto incrementing for it's element id or name
                        if ( autoIncrement !== undefined && autoIncrement === "1" )
                        {
                            // Get elements total count
                            total = target.find( '*[data-increment="1"]' ).length;

                            // Great, sneak through the element name or id and increment
                            html = clone.html().replace( /\[0]/g, "[" + total + "]" ).replace( /\_0_/g, "_" + total + "_" );
                            clone.html( html );

                            /*clone.find("img").attr("src", "");
                            clone.find("a").attr("href", "#");*/
                            clone.find( 'input[type="file"],input[type="text"],input[type="hidden"],textarea,select' ).val( "" );

                            clone.find( "#invoice-details, #writeoff-details" ).css( "display", "none" );
                            clone.find( "#invoice-details, #writeoff-details" ).each( function ()
                            {
                                $( this )
                                    .find( 'input[type="file"],input[type="text"],input[type="hidden"],textarea,select' )
                                    .removeAttr( "required" );
                            } );
                        }

                        // Append clone to the defined target like so:
                        clone.appendTo( target );
                    }

                    // Restart JT JS DOM
                    //ACT.Init.Start();

                    return false;
                } );
            } );
        },

        DataDelOneMore: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );
                var remove = $( i.attr( "data-target" ) );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        if ( !remove.length ) return;

                        if ( $( "#doc-upload" ).find( ".grouped-area" ).length < 2 ) return;

                        remove.hide( 1200, function ()
                        {
                            $( this ).parent().remove();

                            $( "#doc-upload" ).find( ".grouped-area" ).each( function ( i )
                            {
                                ACT.UI.DataIndex( $( this ).find( 'input,select,textarea,label,a,div[data-del-holder="1"]' ), i );
                            } );

                            ACT.UI.DataDelOneMore( $( '*[data-del-one-more="1"]' ) );
                        } );
                    } );
            } );
        },

        DataModal: function ( params )
        {
            params.each( function ()
            {
                var i = $( this );

                i
                    .unbind( 'click' )
                    .bind( 'click', function ()
                    {
                        if ( ACT.Modal.MovedObj.length )
                        {
                            ACT.Modal.MovedObj.appendTo( ACT.Modal.MovedObjSource );
                        }

                        $( ACT.Modal.Container ).find( '#modal-body' ).html( '' );
                        $( ACT.Modal.Container ).find( '#modal-title' ).html( '' );

                        var title = i.attr( 'data-title' );
                        var data = $( i.attr( 'data-target' ) );

                        ACT.Modal.MovedObj = data.children();
                        ACT.Modal.MovedObjSource = data;

                        data.children().appendTo( $( ACT.Modal.Container ).find( '#modal-body' ) );

                        ACT.Modal.Open( null, title );

                        ACT.StartUp.Start();

                        return false;
                    } );
            } );
        },

        DataStickyOne: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var t = i.attr( "data-t" );
                var arrow = i.attr( "data-arrow" );
                var trigger = i.attr( "data-trigger" );
                var callback = i.attr( "data-callback" );
                var title = i.attr( "data-title" ) || i.attr( "original-title" );

                var target = $( i.attr( "data-target" ) );

                if ( !target.length ) return;

                i
                    .unbind( trigger )
                    .bind( trigger, function ()
                    {
                        $( ".tipsy" ).remove();
                        ACT.Sticky.Show( i, title, target.html(), [], arrow );

                        ACT.Init.PluginInit( ACT.Sticky.StickyOne );
                        ACT.UI.DataShowSelected( $( '*[data-show-selected="1"]' ) );

                        if ( typeof ( callback ) === typeof ( Function ) )
                        {
                            try
                            {
                                callback();
                            }
                            catch ( e )
                            {
                                eval( callback );
                            }
                        }
                        else
                        {
                            eval( callback );
                        }
                    } );
            } );
        },

        DataCustomSearch: function ( sender, target )
        {
            var t = sender.attr( "data-t" ).replace( "#", "" );

            target.each( function ()
            {
                var f = $( this );

                var form = f.find( "form" );

                ACT.UI.RecreatePlugins( f );

                f.animate( { "opacity": "1", "filter": "alpha(opacity=100)" }, 1000, function () { } );

                ACT.UI[t].PageSkip = 0;
                ACT.UI[t].PageNumber = 0;

                ACT.Init.AppendPaging( form, t );
            } );
        },

        RecreatePlugins: function ( sender )
        {
            // Destroy any select 2
            sender.find( "div.chzn" ).remove();
            sender.find( "select.chzn" ).css( "display", "block" );

            sender.find( "select.chzn, input.date-picker" ).each( function ( i )
            {
                $( this ).removeClass( "hasDatepicker" );
                $( this ).attr( "id", $( this ).attr( "id" ) + "_" + i );
            } );

            ACT.Init.PluginInit( sender );
        },

        DataDoCustomSearch: function ( sender, target, url, callback )
        {
            // Params
            var params = ACT.UI.GetCustomSearchParams( target.attr( "id" ) );

            ACT.UI.Get( sender, target, url, params, callback, true );

            ACT.UI.SelectedItems = [];

            return false;
        },

        GetCustomSearchParams: function ( t )
        {
            // Params
            var params = {
                Skip: ACT.UI[t].PageSkip || ACT.UI.PageSkip || 0,
                Take: ACT.UI[t].PageLength || ACT.UI.PageLength || 50,
                Page: ACT.UI[t].PageNumber || ACT.UI.PageNumber || 0,
                Sort: ACT.UI[t].PageSort || ACT.UI.PageSort || "ASC",
                SortBy: ACT.UI[t].PageSortBy || ACT.UI.PageSortBy || "Id",
                UserId: ACT.UI[t].PageUserId || ACT.UI.PageUserId || 0,
                Status: ACT.UI[t].PageStatus || ACT.UI.PageStatus || 0,
                PSPClientStatus: ACT.UI[t].PagePSPClientStatus || ACT.UI.PagePSPClientStatus || 0,
                SiteId: ACT.UI[t].PageSiteId || ACT.UI.PageSiteId || 0,
                ClientId: ACT.UI[t].PageClientId || ACT.UI.PageClientId || 0,
                ProductId: ACT.UI[t].PageProductId || ACT.UI.PageProductId || 0,
                CampaignId: ACT.UI[t].PageCampaignId || ACT.UI.PageCampaignId || 0,
                SelectedItems: ACT.UI[t].SelectedItems || ACT.UI.SelectedItems || [],
                FromDate: ACT.UI[t].PageFromDate || ACT.UI.PageFromDate || "",
                ToDate: ACT.UI[t].PageToDate || ACT.UI.PageToDate || "",
                ActionDate: ACT.UI[t].PageActionDate || ACT.UI.PageActionDate || "",
                Bank: ACT.UI[t].PageBankId || ACT.UI.PageBankId || -1,
                Account: ACT.UI[t].PageAccount || ACT.UI.PageAccount || "",
                AccountType: ACT.UI[t].PageAccountType || ACT.UI.PageAccountType || "",
                DocumentType: ACT.UI[t].PageDocumentType || ACT.UI.PageDocumentType || "",
                VAT: ACT.UI[t].PageVAT || ACT.UI.PageVAT || false,
                ActivityType: ACT.UI[t].PageActivityType || ACT.UI.PageActivityType || -1,
                RoleType: ACT.UI[t].PageRoleType || ACT.UI.PageRoleType || -1,
                Province: ACT.UI[t].PageProvince || ACT.UI.PageProvince || -1,
                City: ACT.UI[t].PageCity || ACT.UI.PageCity || "",
                Query: ACT.UI[t].PageQuery || ACT.UI.PageQuery || "",
                ReturnView: ACT.UI[t].PageReturnView || ACT.UI.PageReturnView || "",
                Controller: ACT.UI[t].PageController || ACT.UI.PageController || "",
                IsCustomSearch: ACT.UI[t].IsCustomSearch || ACT.UI.IsCustomSearch || false
            };

            return params;
        },

        ClearCustomSearch: function ( t )
        {
            if ( t === "" ) return;

            if ( !ACT.UI[t] )
            {
                ACT.UI[t] = [];
            }

            ACT.UI[t].PageSkip = ACT.UI.PageSkip = 0;
            ACT.UI[t].PageNumber = ACT.UI.PageNumber = 1;
            ACT.UI[t].PageLength = ACT.UI.PageLength = 50;
            ACT.UI[t].PageSort = ACT.UI.PageSort = "ASC";
            ACT.UI[t].PageSortBy = ACT.UI.PageSortBy = "Id";
            ACT.UI[t].PageUserId = ACT.UI.PageUserId = 0;
            ACT.UI[t].PageSiteId = ACT.UI.PageSiteId = 0;
            ACT.UI[t].PageClientId = ACT.UI.PageClientId = 0;
            ACT.UI[t].PageProductId = ACT.UI.PageProductId = 0;
            ACT.UI[t].PageCampaignId = ACT.UI.PageCampaignId = 0;
            ACT.UI[t].PageStatus = ACT.UI.PageStatusId = 0;
            ACT.UI[t].PagePSPClientStatus = ACT.UI.PagePSPClientStatus = 0;
            ACT.UI[t].SelectedItems = ACT.UI.SelectedItems = [];
            ACT.UI[t].PageFromDate = ACT.UI.PageFromDate = "";
            ACT.UI[t].PageToDate = ACT.UI.PageToDate = "";
            ACT.UI[t].PageActionDate = ACT.UI.PageActionDate = "";
            ACT.UI[t].PageBankId = ACT.UI.PageBankId = -1;
            ACT.UI[t].PageAccount = ACT.UI.PageAccount = "";
            ACT.UI[t].PageAccountType = ACT.UI.PageAccountType = "";
            ACT.UI[t].PageDocumentType = ACT.UI.PageDocumentType = "";
            ACT.UI[t].PageVAT = false;
            ACT.UI[t].PageActivityType = ACT.UI.PageActivityType = -1;
            ACT.UI[t].PageRoleType = ACT.UI.PageRoleType = -1;
            ACT.UI[t].PageProvince = ACT.UI.PageProvince = -1;
            ACT.UI[t].PageQuery = ACT.UI.PageQuery = "";
            ACT.UI[t].PageQuery = ACT.UI.PageQuery = "";
            ACT.UI[t].PageReturnView = ACT.UI.PageReturnView = "_List";
            ACT.UI[t].PageController = ACT.UI.PageController = "DashBoard";
            ACT.UI[t].IsCustomSearch = ACT.UI.IsCustomSearch = false;

            return true;
        },

        BeginCustomSearch: function ( sender )
        {
            ACT.Loader.Show( sender.find( "#save-btn" ), true );

            if ( sender.find( "#ReturnView" ).length )
            {
                var t = sender.find( "#ReturnView" ).val().replace( "_", "" ).toLowerCase();

                ACT.UI[t] = ACT.UI[t] || [];

                sender.find( 'select,textarea,input[type="text"],input[type="checkbox"],input[type="hidden"]' ).each( function ()
                {
                    var i = $( this );

                    var id = i.attr( "id" );

                    if ( typeof ( id ) === "undefined" ) return;

                    id = "Page" + id.split( "_" )[0];

                    ACT.UI[t][id] = i.val();

                    if ( $( this ).is( ":checkbox" ) || $( this ).is( ":radio" ) )
                    {
                        ACT.UI[t][id] = $( this ).is( ":checked" );
                    }
                } );

                ACT.UI[t].IsCustomSearch = true;
            }
        },

        CompleteCustomSearch: function ( sender )
        {
            ACT.Sticky.Hide();
            ACT.Init.Start( true );
        },

        DataHighlightFields: function ( target )
        {
            target
                .find( ".display-field, .editor-field input, .editor-field select, .editor-field textarea" )
                .css( "background", "#002e70" )
                .animate
                ( {
                    "opacity": "0.1",
                    "filter": "alpha(opacity=10)"
                }, 1000, function ()
                {
                    $( this ).css( "background", "#ffffff" ).animate(
                        {
                            "opacity": "1",
                            "filter": "alpha(opacity=100)"
                        }, 1000 );
                } );
        },

        DataPartialImages: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var rendered = parseInt( i.attr( "data-rendered" ) );

                if ( rendered === 1 ) return;

                var vid = i.attr( "data-vid" );
                var url = i.attr( "data-url" );
                var view = ( i.attr( "data-view" ) === "view" );

                i.attr( "data-rendered", 1 );

                i.append( "<span></span>" );

                ACT.Loader.Show( i.find( "span" ), true );

                $.get( siteurl + url, { vid: vid, view: view }, function ( data, status, req )
                {
                    i.html( data );

                    ACT.Init.Start( true );

                } ).error( function ()
                {

                } ).fail( function ()
                {

                } );
            } );
        },



        DataEdit: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var id = i.attr( "data-id" );
                var target = i.closest( i.attr( "data-target" ) ).first();

                if ( !target.length )
                {
                    target = $( i.attr( "data-target" ) );
                }

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        var url = i.attr( "href" );

                        var load = false;
                        if ( target.parent().is( "div" ) )
                        {
                            load = true;
                            newTarget = target;
                            loader = target.find( "span.loader" );
                        }
                        else
                        {
                            target.parent().find( 'tr.edit' ).remove();

                            var columns = $( 'table.datatable-numberpaging tbody tr:nth-child(1) td' ).length;
                            var row = '<tr id="tr-edit" class="edit ' + target.attr( 'class' ) + '"><td id="editing-td" target="#' + target.attr( 'id' ) + '" colspan="' + columns + '"><span></span></td></tr>';

                            target.after( row );

                            newTarget = target.parent().find( 'tr.edit td' );
                            loader = newTarget.find( "span" );
                        }

                        ACT.UI.Get( loader, newTarget, url, {}, {}, load );

                        return false;
                    } );

            } );
        },

        DataDetails: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var id = i.attr( "data-id" );
                var target = i.closest( i.attr( "data-target" ) ).first();

                if ( !target.length )
                {
                    target = $( i.attr( "data-target" ) );
                }

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        var hasQ = ( i.attr( "href" ).indexOf( '?' ) > -1 );

                        var url = ( hasQ ) ? i.attr( "href" ) + "&layout=false" : i.attr( "href" ) + "?layout=false";

                        target.parent().find( 'tr.edit' ).remove();

                        var columns = target.parent().parent().find( 'tr:nth-child(1) td' ).length;
                        var row = '<tr id="tr-edit" class="edit ' + target.attr( 'class' ) + '"><td colspan="' + columns + '"><span></span></td></tr>';

                        target.after( row );
                        target = target.parent().find( 'tr.edit td' );

                        ACT.UI.Get( target.find( "span" ), target, url, {} );

                        return false;
                    } );
            } );
        },

        DataDelete: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var id = i.attr( "data-id" );
                var target = i.closest( i.attr( "data-target" ) );
                if ( !target.length )
                {
                    target = $( i.attr( "data-target" ) );
                }

                var refresh = i.closest( i.attr( "data-refresh-target" ) );
                if ( !refresh.length )
                {
                    refresh = $( i.attr( "data-refresh-target" ) );
                }

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        ACT.UI.DeleteFix( i, target, refresh );
                        return false;
                    } );
            } );
        },

        DataCancelItem: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var id = i.attr( "data-id" );

                var target = $( i.attr( "data-target" ) );
                var refresh = $( i.attr( "data-refresh" ) );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        var title = "Delete/Cancel this item?";
                        var msg = '<p style="margin: 0;">Are you sure you would like to delete/cancel this item?</p>';

                        var btn = $( ACT.Modal.Container ).find( '.btns #btnConfirm' );

                        btn.val( "Yes" );

                        ACT.Modal.Open( msg, title, true );

                        btn
                            .unbind( "click" )
                            .bind( "click", function ()
                            {
                                var url = sender.attr( "href" );

                                var columns = $( 'table.datatable-numberpaging tbody tr:nth-child(1) td' ).length;
                                var row = '<tr class="edit ' + target.attr( 'class' ) + '"><td colspan="' + columns + '"><span></span></td></tr>';

                                target.parent().find( 'tr.edit' ).remove();

                                target.after( row );

                                ACT.Loader.Show( target.parent().find( 'tr.edit td span' ) );

                                refresh.load( url, { id: id }, function ()
                                {
                                    $( ".tipsy" ).remove();

                                    ACT.Init.Start( true );
                                    ACT.UI.DataHotSpot( $( '*[data-hot-spot="1"]' ), true );
                                } );

                                ACT.Modal.Close();
                            } );

                        return false;
                    } );
            } );
        },

        DeleteFix: function ( sender, target, refresh )
        {
            var url = sender.attr( "href" );

            var columns = $( 'table.datatable-numberpaging tbody tr:nth-child(1) td' ).length;
            var row = '<tr class="edit ' + target.attr( 'class' ) + '"><td colspan="' + columns + '"><span></span></td></tr>';

            target.parent().find( 'tr.edit' ).remove();

            target.after( row );

            ACT.Loader.Show( target.parent().find( 'tr.edit td span' ) );

            url = url + "?query=" + ACT.UI.PageSearch + "&skip=" + ACT.UI.PageSkip + "&take=" + ACT.UI.PageLength + "&page=" + ACT.UI.PageNumber;

            refresh.load( url, {}, function ()
            {
                $( ".tipsy" ).remove();

                ACT.Init.Start( true );
            } );

            return false;
        },

        DataCancel: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var target = i.closest( i.attr( "data-target" ) );
                var remove = i.closest( i.attr( "data-remove" ) );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        target.animate(
                            {
                                "width": "0",
                                "height": "0",
                                "opacity": "0",
                                "filter": "alpha(opacity=0)"
                            }, 700, function ()
                        {
                            remove.remove();
                        } );

                        return false;
                    } );
            } );
        },

        DataFormSubit: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var form = $( i.attr( "data-form" ) );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        var valid = ACT.Validation.Validate( i );

                        if ( valid )
                        {
                            form.submit();
                        }
                        else
                        {
                            form.find( "input" ).each( function ()
                            {
                                var i = $( this );
                                var id = i.attr( "id" );

                                var err = i.attr( "data-error" );
                                var errCntr = $( '*[data-valmsg-for="' + id + '"]' );

                                i.addClass( "input-validation-error" );

                                errCntr.removeClass( "field-validation-valid" );
                                errCntr.addClass( "field-validation-error" );

                                errCntr.html( '<span for="' + i.attr( "id" ) + '" generated="true">' + err + '</span>' );
                            } );
                        }

                        return false;
                    } );
            } );
        },

        DataPRSum: function ( sender, force )
        {
            sender.each( function ()
            {
                var i = $( this );
                var papa = i.parent();
                var total = papa.find( ".pr-total" );

                if ( typeof i.attr( "data-loaded" ) !== undefined && i.attr( "data-loaded" ) === "1" && !force ) return;

                var j = { status: i.attr( "data-status" ), status2: i.attr( "data-status2" ), method: i.attr( "data-method" ) };

                var h = "<img style='width: 20px;' title='Hello, just busy updating this menu total.' alt='' src='" + imgurl + "/images/hot.gif' />";

                i.add( total ).append( h ).show( 1200 );

                $.ajax( {
                    url: siteurl + "/PRSum",
                    type: "POST",
                    data: JSON.stringify( j ),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    error: function ( response )
                    {
                        i.find( ".hot-spot" ).fadeOut( 1200 );
                    },
                    success: function ( response )
                    {
                        i.html( "R " + response.sum.money( 2 ) );
                        total.html( response.total );

                        i.attr( "data-loaded", "1" );
                    }
                } );
            } );
        },


        /** Table Quick Links Operations **/

        DataCheckOptions: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );
                var papa = $( i.parent() );
                var mother = $( i.attr( "data-mother" ) );
                var mykids = $( i.attr( "data-my-kids" ) );

                var main = parseInt( i.attr( "data-main" ) );

                i.unbind( 'change keyup' );
                i.bind( 'change keyup', function ()
                {
                    if ( ( i.is( ':checkbox' ) && i.is( ':checked' ) ) || ( i.is( ':text' ) && i.val() !== "" ) )
                    {
                        if ( main === 0 )
                        {
                            papa.removeClass( 'message-warn' );
                            papa.addClass( 'message-success' );

                            //mother.prop("checked", true);
                        }
                        else
                        {
                            //mykids.prop("checked", true);

                            //mykids.parent().removeClass('message-warn');
                            //mykids.parent().addClass('message-success');
                            papa.parent().removeClass( 'message-warn' );
                            papa.parent().addClass( 'message-success' );

                        }
                    }
                    else
                    {
                        if ( main === 0 )
                        {
                            papa.addClass( 'message-warn' );
                            papa.removeClass( 'message-success' );
                        }
                        else
                        {
                            //mykids.removeAttr("checked");

                            //mykids.parent().addClass('message-warn');
                            //mykids.parent().removeClass('message-success');

                            if ( papa.parent().find( 'input[type="hidden"]' ).length && papa.parent().find( 'input[type="hidden"]' ).val() !== "0" )
                            {
                                i.prop( "checked", true ).attr( "checked", "checked" );

                                var h = papa.parent().find( 'input[type="hidden"]' );

                                var title = h.attr( "data-title" );
                                var message = '<p>' + h.attr( "data-message" ) + '</p>';

                                message += '<p>';
                                message += '  <input id="yes-remove" value="Yes" type="button" class="yes-btn" />';
                                message += '  <span style="padding: 0 5px;">/</span>';
                                message += '  <input id="no-remove" value="NO!" type="button" class="no-btn" />';
                                message += '<p>';

                                ACT.Sticky.Show( i, title, message, [], "center-left" );

                                var no = ACT.Sticky.StickyOne.find( "#no-remove" );
                                var yes = ACT.Sticky.StickyOne.find( "#yes-remove" );

                                no
                                    .unbind( "click" )
                                    .bind( "click", function ()
                                    {
                                        ACT.Sticky.Hide();
                                    } );

                                yes
                                    .unbind( "click" )
                                    .bind( "click", function ()
                                    {
                                        ACT.Loader.Show( yes, true );

                                        $.post( ACT.UI.URL + h.attr( "data-url" ), { id: h.val() }, function ( data )
                                        {
                                            ACT.Loader.Hide();

                                            var d = $( "<div/>" ).html( data );

                                            ACT.Sticky.Show( i, d.find( ".title" ).text(), d.find( ".message" ).html(), [], "center-left" );

                                            // Clear date values, if any...
                                            papa.parent().find( ".date-picker" ).val( "" );
                                            i.prop( "checked", false ).removeAttr( "checked" );

                                            h.val( 0 );
                                            i.change();
                                        } );
                                    } );

                                return false;
                            }

                            papa.parent().removeClass( 'message-success' );
                            papa.parent().addClass( 'message-warn' );
                        }
                    }
                } );
            } );
        },



        /** PRIVATE LOCAL FUNCTIONS **/

        Get: function ( sender, target, url, params, callback, loadImg, noAnminate )
        {
            loadImg = loadImg ? true : false;

            ACT.Loader.Show( sender, loadImg );

            $.get( url, params, function ( data, s, xhr )
            {
                if ( s === "error" )
                {
                    ACT.Modal.Open( xhr.responseText, xhr.statusText, false, ACT.Init.Start() );

                    return;
                }

                target.html( data );

                // Update form data-target="#..."
                //var form = target.find( "form" );

                //if ( form.length && target.closest( ".da-tab" ).length )
                //{
                //    form.attr( "data-target", "#" + target.closest( ".da-tab" ).attr( "id" ) );
                //    form.append( '<input type="hidden" value="' + target.closest( ".da-tab" ).attr( "data-load-url" ) + '" name="ReturnView">' );
                //}

                ACT.Init.Start( true );

                $.validator.unobtrusive.parse( target );

                if ( noAnminate === undefined )
                {
                    $( 'html, body' ).animate( { scrollTop: target.offset().top - 60 }, 'slow', function () { } );
                }

                if ( target.find( ".dataTables_wrapper" ).length )
                {
                    ACT.UI.DataTablesOverride( target );
                }

                ACT.UI.DataCallBack( callback );

            } );
        },

        Post: function ( sender, target, url, params, callback, loadImg, noAnminate )
        {
            loadImg = loadImg ? true : false;

            ACT.Loader.Show( sender, loadImg );

            $.post( url, params, function ( data, s, xhr )
            {
                if ( s === "error" )
                {
                    ACT.Modal.Open( xhr.responseText, xhr.statusText, false, ACT.Init.Start() );

                    return;
                }

                target.html( data );

                // Update form data-target="#..."
                //var form = target.find( "form" );

                //if ( form.length && target.closest( ".da-tab" ).length )
                //{
                //    form.attr( "data-target", "#" + target.closest( ".da-tab" ).attr( "id" ) );
                //    form.append( '<input type="hidden" value="' + target.closest( ".da-tab" ).attr( "data-load-url" ) + '" name="ReturnView">' );
                //}

                ACT.Init.Start( true );

                $.validator.unobtrusive.parse( target );

                if ( noAnminate === 'undefined' )
                {
                    $( 'html, body' ).animate( { scrollTop: target.offset().top - 60 }, 'slow', function () { } );
                }

                ACT.UI.DataCallBack( callback );
            } );
        },

        /** PRIVATE LOCAL FUNCTIONS **/



        /** PLUGIN OVERRIDES **/

        AfterSort: function ()
        {
            var dt = ( $( '#tab-data>div:visible' ).length ) ? $( '#tab-data>div:visible' ) : ( $( '#collapse>div:visible' ).length ) ? $( '#collapse>div:visible' ) : ( $( "#list" ).length ) ? $( "#list" ) : $( '#item-list' );

            if ( dt.find( "#collapse" ).length )
            {
                dt = dt.find( "#collapse>div:visible" );
            }

            var t = dt.attr( "id" );

            var th = dt.find( 'th[data-column="' + ACT.UI[t].PageSortBy + '"]' );

            th.attr( "data-sort", ACT.UI[t].PageSort )
                .removeClass( "sorting" )
                .addClass( "sorting_" + ACT.UI[t].PageSort );
        },

        DataTablesOverride: function ( sender )
        {
            var t = "";

            sender.each( function ()
            {
                var i = $( this );

                t = i.attr( "id" );

                ACT.UI[t] = ACT.UI[t] || { PageLength: 50 };

                // Hide Defaults
                i.find( ".dataTables_wrapper .dataTables_length,.dataTables_wrapper .dataTables_info,.dataTables_wrapper .dataTables_filter,.dataTables_wrapper .dataTables_paginate" ).remove();

                if ( i.find( ".tiny" ).length > 0 ) return;

                // Overrides

                // 1. Page Length
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var length = i.find( "#page-length" );
                var l_cntr = length.find( "#data-page-length" );

                l_cntr.val( ACT.UI[t].PageLength );

                if ( !i.find( ".dataTables_wrapper #page-length" ).length )
                {
                    i.find( ".dataTables_wrapper" ).prepend( length );
                }

                l_cntr
                    .unbind( "change" )
                    .bind( "change", function ()
                    {
                        ACT.UI[t].PageLength = l_cntr.val();

                        // Reset
                        ACT.UI[t].PageSkip = 0;
                        ACT.UI[t].PageNumber = 0;

                        var url = ( siteurl + l_cntr.attr( "data-url" ) ).split( '?' )[0].replace( siteurl, "" );

                        // Params
                        if ( ACT.UI[t].IsCustomSearch )
                        {
                            return ACT.UI.DataDoCustomSearch( l_cntr, i, url, ACT.UI.AfterSort );
                        }

                        var params = ACT.UI.GetCustomSearchParams( t );

                        params.Page = 0;
                        params.Skip = 0;
                        params.Take = l_cntr.val();
                        params.Query = ACT.UI[t].PageSearch;

                        ACT.UI.Get( l_cntr, i, url, params, ACT.UI.AfterSort, true );
                    } );

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////




                // 2. Page Search
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var search = i.find( "#page-search" );
                var s_cntr = search.find( "#data-page-search" );

                if ( !i.find( ".dataTables_wrapper #page-search" ).length )
                {
                    i.find( ".dataTables_wrapper" ).prepend( search );
                }
                if ( ACT.UI[t].PageSearch )
                {
                    s_cntr.focus().val( ACT.UI[t].PageSearch );
                }
                else
                {
                    s_cntr.val( "" );
                }

                // Check if seach values loaded?
                var s_target = $( s_cntr.attr( "data-target" ) );
                if ( !s_target.length && !i.find( "#givecsm" ).length && s_cntr.hasClass( "do-custom-search" ) )
                {
                    i.append( "<div id='givecsm'></div>" );

                    s_target = i.find( "#givecsm" );

                    var by = ACT.UI[t].PageBudgetYear;

                    if ( ( typeof by === 'undefined' || by <= 0 ) && parseInt( ACT.UI.DataGetQueryString( "BudgetYear" ) ) > 0 )
                    {
                        by = ACT.UI.DataGetQueryString( "BudgetYear" );
                    }

                    s_target.load( siteurl + "/" + s_cntr.attr( "data-t" ), { givecsm: true, bYear: by }, function ()
                    {
                        ACT.UI.DataCustomSearchHighlight( sender, t );
                        ACT.UI.DataStickyOne( $( '*[data-sticky-one="1"]' ) );
                    } );
                }


                s_cntr
                    .unbind( "keyup" )
                    .bind( "keyup", function ( e )
                    {
                        var enter = ( e.keyCode === 13 || e.which === 13 || ( $( this ).val() === "" ) );

                        if ( enter )
                        {
                            ACT.UI[t].PageSearch = ACT.UI[t].PageQuery = s_cntr.val();

                            var url = ( siteurl + s_cntr.attr( "data-url" ) ).split( '?' )[0].replace( siteurl, "" );

                            if ( ACT.UI[t].IsCustomSearch )
                            {
                                ACT.UI[t].PageQuery = s_cntr.val();

                                return ACT.UI.DataDoCustomSearch( s_cntr, i, url, ACT.UI.AfterSort );
                            }

                            var params = ACT.UI.GetCustomSearchParams( t );

                            params.Page = 0;
                            params.Skip = 0;
                            params.Take = ACT.UI[t].PageLength;
                            params.Query = s_cntr.val();

                            ACT.UI.Get( s_cntr, i, url, params, ACT.UI.AfterSort, true );
                        }
                    } );

                //icon
                //.unbind("click")
                //.bind("click", function ()
                //{
                //    ACT.UI[t].PageSearch = ACT.UI[t].PageQuery = s_cntr.val();

                //    var url = (siteurl + s_cntr.attr("data-url")).split('?')[0].replace(siteurl, "");

                //    if (ACT.UI[t].IsCustomSearch)
                //    {
                //        ACT.UI[t].PageQuery = s_cntr.val();

                //        return ACT.UI.DataDoCustomSearch(s_cntr, i, url, ACT.UI.AfterSort);
                //    }

                //    ACT.UI.Get(s_cntr, i, url, { query: s_cntr.val(), skip: 0, take: ACT.UI[t].PageLength, page: 0 }, ACT.UI.AfterSort, true);
                //});

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////




                // 3. Page Count
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var count = i.find( "#page-count" );

                if ( !i.find( ".dataTables_wrapper #page-count" ).length )
                {
                    i.find( ".dataTables_wrapper" ).append( count );
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////




                // 4. Page Navigation (Indexing)
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var navigation = i.find( "#page-navigation" );

                var next_cntr = navigation.find( "#data-page-nav-next" );
                var previous_cntr = navigation.find( "#data-page-nav-previous" );

                if ( !i.find( ".dataTables_wrapper #page-navigation" ).length )
                {
                    i.find( ".dataTables_wrapper" ).append( navigation );
                }

                next_cntr.add( previous_cntr ).each( function ()
                {
                    var n = $( this );

                    n
                        .unbind( "click" )
                        .bind( "click", function ()
                        {
                            if ( n.hasClass( "inactive" ) ) return;

                            var skip = parseInt( n.attr( "data-skip" ) );
                            var page = parseInt( n.attr( "data-page" ) );

                            ACT.UI[t].PageSkip = skip;
                            ACT.UI[t].PageNumber = page;

                            var url = ( siteurl + navigation.attr( "data-url" ) ).split( '?' )[0].replace( siteurl, "" );

                            if ( ACT.UI[t].IsCustomSearch )
                            {
                                return ACT.UI.DataDoCustomSearch( n, i, url, ACT.UI.AfterSort );
                            }

                            var params = ACT.UI.GetCustomSearchParams( t );

                            params.Page = page;
                            params.Skip = skip;
                            params.Take = ACT.UI[t].PageLength;
                            params.Query = ACT.UI[t].PageSearch;

                            ACT.UI.Get( n, i, url, params, ACT.UI.AfterSort, true );
                        } );
                } );

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////




                // 5. Sorting override
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////

                i.find( 'th[data-column]' ).each( function ()
                {
                    var th = $( this );
                    var tr = th.parent( "tr" );
                    var column = th.attr( "data-column" );

                    if ( typeof ( column ) === 'undefined' )
                    {
                        th
                            .unbind( "click" )
                            .removeClass( "sorting sorting_asc sorting_desc" );

                        return;
                    }

                    th.removeClass( "sorting_disabled" ).addClass( "sorting" );

                    th
                        .unbind( "click" )
                        .bind( "click", function ()
                        {
                            tr.find( 'th[data-column]' )
                                .removeClass( "sorting_asc sorting_desc" )
                                .addClass( "sorting" );

                            var sort = th.attr( "data-sort" );

                            if ( typeof ( sort ) === 'undefined' )
                            {
                                sort = "asc";
                            }
                            else
                            {
                                sort = ( sort === "asc" ) ? "desc" : "asc";
                            }

                            ACT.UI[t].PageSort = sort;
                            ACT.UI[t].PageSortBy = column;

                            var url = ( siteurl + navigation.attr( "data-url" ) ).split( '?' )[0].replace( siteurl, "" );

                            if ( ACT.UI[t].IsCustomSearch )
                            {
                                return ACT.UI.DataDoCustomSearch( $( "#sort-loader" ), i, url, ACT.UI.AfterSort );
                            }

                            var params = ACT.UI.GetCustomSearchParams( t );

                            params.Skip = 0;
                            params.Page = 0;
                            params.Take = ACT.UI[t].PageLength;
                            params.Query = ACT.UI[t].PageSearch;

                            params.Sort = sort;
                            params.SortBy = column;

                            ACT.UI.Get( $( "#sort-loader" ), i, url, params, ACT.UI.AfterSort, true );
                        } );
                } );

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////




                // 6. Table Totals
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////

                setTimeout( function ()
                {
                    var totals = i.find( "#table-totals" );
                    var label = i.find( "td.total-label" ).last();
                    var field = i.find( "td.total-field" ).last();

                    if ( totals.length && totals.length && label.length && totals.find( "#total-label" ).length && totals.find( "#total-field" ).length )
                    {
                        totals.find( "#total-label" ).appendTo( i.find( ".dataTables_wrapper" ) )
                            .css( { left: label.position().left, width: label.outerWidth() } )
                            .show( 1200 );

                        totals.find( "#total-field" ).appendTo( i.find( ".dataTables_wrapper" ) )
                            .css( { left: field.position().left, width: field.outerWidth() } )
                            .show( 1200 );
                    }
                }, "1500" );

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////




                // 7. Fixed Header
                //////////////////////////////////////////////////////////////////////////////////////////////////////////////

                var table = i.find( "table" );
                var target = $( table.attr( "data-fixed-header-target" ) );

                if ( table.hasClass( "fixed-header" ) && !target.find( "table.fixedHeader-floating" ).length )
                {
                    //
                    var clone = table.clone();

                    // Remove tbody
                    clone.find( "tbody" ).remove();

                    var leway = -1;

                    var sp = $( table.attr( "data-starting-point" ) );
                    var spHeight = sp.outerHeight();

                    clone
                        .addClass( "none" )
                        .removeClass( "fixed-header" )
                        .addClass( "fixedHeader-floating" )
                        .attr( "id", table.attr( "id" ) + "-fixed-header" )
                        .css( { top: ( spHeight + leway ), left: table.position().length, width: table.outerWidth(), "table-layout": "fixed" } );

                    target.append( clone );

                    $( window )
                        .bind( "scroll resize", function ()
                        {
                            var header = target.find( "table.fixedHeader-floating" );

                            ACT.UI.ShowFixedHeader( header, table );

                            /*var scrolled = $( document ).scrollTop();

                            if ( scrolled > ( spHeight + 22 ) )
                            {
                                header.find( "th" ).each( function ()
                                {
                                    $( this )
                                        .removeAttr( "style" )
                                        .css( "width", table.find( 'th[data-name="' + $( this ).attr( "data-name" ) + '"]' ).width() );
                                } );

                                header.css( { "display": "block", "width": table.outerWidth() } );
                            }
                            else if ( scrolled < ( spHeight + 22 ) && header.is( ":visible" ) )
                            {
                                header.css( "display", "none" );
                            }*/
                        } );
                }

                //////////////////////////////////////////////////////////////////////////////////////////////////////////////


            } );

            //ACT.UI.DataCustomSearchHighlight(sender, t);
        },

        ShowFixedHeader: function ( header, table )
        {
            var sp = $( table.attr( "data-starting-point" ) );
            var spHeight = sp.outerHeight();

            var scrolled = $( document ).scrollTop();

            if ( scrolled > ( spHeight + 22 ) )
            {
                header.find( "th" ).each( function ()
                {
                    $( this )
                        .removeAttr( "style" )
                        .css( "width", table.find( 'th[data-name="' + $( this ).attr( "data-name" ) + '"]' ).width() );
                } );

                header.css( { "display": "block", "width": table.outerWidth() } );
            }
            else if ( scrolled < ( spHeight + 22 ) )
            {
                header.css( "display", "none" );
            }
        },

        DataTablesDateRange: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                // Hide Defaults
                i.find( ".dataTables_wrapper .dataTables_length,.dataTables_wrapper .dataTables_info,.dataTables_wrapper .dataTables_filter,.dataTables_wrapper .dataTables_paginate" ).remove();

                var dateRange = i.find( "#date-range" );

                dateRange.css( "display", "block" );
                i.find( ".dataTables_wrapper" ).prepend( dateRange );

                var end = dateRange.find( '[data-end-date="1"]' );
                var start = dateRange.find( '[data-start-date="1"]' );

                end.add( start ).each( function ()
                {
                    var dr = $( this );

                    dr
                        .unbind( "change keyup" )
                        .bind( "change keyup", function ()
                        {
                            i.find( "table a" ).each( function ()
                            {
                                var a = $( this );

                                if ( typeof ( a.attr( "original-href" ) ) === "undefined" )
                                {
                                    a.attr( "original-href", a.attr( "href" ) );
                                }

                                var href = a.attr( "original-href" );

                                a.attr( "href", href + "&StartDate=" + start.val() + "&EndDate=" + end.val() );
                            } );
                        } );
                } );
            } );
        },

        DataCustomSearchHighlight: function ( sender, t )
        {
            var ics = false;

            if ( ACT.UI[t] && ACT.UI[t].IsCustomSearch )
            {
                ics = true;

                // Guess search criteria
                var q = "", h = "";
                q += "Custom search for: ";

                if ( ACT.UI[t].PageUserId && ACT.UI[t].PageUserId !== 0 )
                {
                    h += "User: <b>" + sender.find( 'select#UserId:first option[value="' + ACT.UI[t].PageUserId + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ User: <a style='color: #69f95a;'>" + sender.find( 'select#UserId:first option[value="' + ACT.UI[t].PageUserId + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#UserId' ).val( ACT.UI[t].PageUserId );
                }
                if ( ACT.UI[t].PageSiteId && ACT.UI[t].PageSiteId !== 0 )
                {
                    h += "Client: <b>" + sender.find( 'select#SiteId:first option[value="' + ACT.UI[t].PageSiteId + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ Site: <a style='color: #69f95a;'>" + sender.find( 'select#SiteId:first option[value="' + ACT.UI[t].PageSiteId + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#SiteId' ).val( ACT.UI[t].PageSiteId );
                }
                if ( ACT.UI[t].PageClientId && ACT.UI[t].PageClientId !== 0 )
                {
                    h += "Client: <b>" + sender.find( 'select#ClientId:first option[value="' + ACT.UI[t].PageClientId + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ Client: <a style='color: #69f95a;'>" + sender.find( 'select#ClientId:first option[value="' + ACT.UI[t].PageClientId + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#ClientId' ).val( ACT.UI[t].PageClientId );
                }
                if ( ACT.UI[t].PageProductId && ACT.UI[t].PageProductId !== 0 )
                {
                    h += "Product: <b>" + sender.find( 'select#ProductId:first option[value="' + ACT.UI[t].PageProductId + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ Product: <a style='color: #69f95a;'>" + sender.find( 'select#ProductId:first option[value="' + ACT.UI[t].PageProductId + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#ProductId' ).val( ACT.UI[t].PageProductId );
                }
                if ( ACT.UI[t].PageCampaignId && ACT.UI[t].PageCampaignId !== 0 )
                {
                    h += "Campaign: <b>" + sender.find( 'select#CampaignId:first option[value="' + ACT.UI[t].PageCampaignId + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ Campaign: <a style='color: #69f95a;'>" + sender.find( 'select#CampaignId:first option[value="' + ACT.UI[t].PageCampaignId + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#CampaignId' ).val( ACT.UI[t].PageCampaignId );
                }
                if ( ACT.UI[t].PageStatus && ACT.UI[t].PageStatus !== 0 )
                {
                    h += "User: <b>" + sender.find( 'select#Status:first option[value="' + ACT.UI[t].PageStatus + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ Status: <a style='color: #69f95a;'>" + sender.find( 'select#Status:first option[value="' + ACT.UI[t].PageStatus + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#Status' ).val( ACT.UI[t].PageStatus );
                }
                if ( ACT.UI[t].PagePSPClientStatus && ACT.UI[t].PagePSPClientStatus !== 0 )
                {
                    h += "User: <b>" + sender.find( 'select#PSPClientStatus:first option[value="' + ACT.UI[t].PagePSPClientStatus + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ Status: <a style='color: #69f95a;'>" + sender.find( 'select#PSPClientStatus:first option[value="' + ACT.UI[t].PagePSPClientStatus + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#PSPClientStatus' ).val( ACT.UI[t].PagePSPClientStatus );
                }
                // Date From & To
                if ( ( ACT.UI[t].PageFromDate || ACT.UI[t].PageToDate ) && ( ACT.UI[t].PageFromDate !== "" || ACT.UI[t].PageToDate !== "" ) )
                {
                    h += "Date: From <b>" + ACT.UI[t].PageFromDate + ( ( ACT.UI[t].PageToDate !== "" ) ? "</b> To <b>" + ACT.UI[t].PageToDate : "" ) + "</b>~";
                    q += " <b class='italic'>[ Date: <a style='color: #69f95a;'>From " + ACT.UI[t].PageFromDate + ( ( ACT.UI[t].PageToDate !== "" ) ? " To " + ACT.UI[t].PageToDate : "" ) + "</a> ]</b> ";

                    sender.find( 'input#ToDate' ).val( ACT.UI[t].PageToDate );
                    sender.find( 'input#FromDate' ).val( ACT.UI[t].PageFromDate );
                }

                // DocumentType
                if ( ACT.UI[t].PageDocumentType && ACT.UI[t].PageDocumentType !== -1 )
                {
                    h += "Document Type: <b>" + sender.find( 'select#DocumentType:first option[value="' + ACT.UI[t].PageDocumentType + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ Document Type: <a style='color: #69f95a;'>" + sender.find( 'select#DocumentType:first option[value="' + ACT.UI[t].PageDocumentType + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#DocumentType' ).val( ACT.UI[t].PageDocumentType );
                }

                // AccountType
                if ( ACT.UI[t].PageAccountType && ACT.UI[t].PageAccountType !== -1 )
                {
                    val = ( sender.find( 'select#AccountType' ).length <= 0 ) ? ACT.UI[t].PageAccountType : sender.find( 'select#AccountType:first option[value="' + ACT.UI[t].PageAccountType + '"]' ).text();

                    h += "Account Type: <b>" + val + "</b>~";
                    q += " <b class='italic'>[ Account Type: <a style='color: #69f95a;'>" + val + "</a> ]</b> ";

                    sender.find( 'select#AccountType' ).val( ACT.UI[t].PageAccountType );
                }

                // RoleType
                if ( ACT.UI[t].PageRoleType && ACT.UI[t].PageRoleType !== -1 )
                {
                    h += "Role Type: <b>" + sender.find( 'select#RoleType:first option[value="' + ACT.UI[t].PageRoleType + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ Role Type: <a style='color: #69f95a;'>" + sender.find( 'select#RoleType:first option[value="' + ACT.UI[t].PageRoleType + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#RoleType' ).val( ACT.UI[t].PageRoleType );
                }

                // ActivityType
                if ( ACT.UI[t].PageActivityType && ACT.UI[t].PageActivityType !== -1 )
                {
                    h += "Activity Type: <b>" + sender.find( 'select#ActivityType:first option[value="' + ACT.UI[t].PageActivityType + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ Activity Type: <a style='color: #69f95a;'>" + sender.find( 'select#ActivityType:first option[value="' + ACT.UI[t].PageActivityType + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#ActivityType' ).val( ACT.UI[t].PageRoleType );
                }

                // Province
                if ( ACT.UI[t].PageProvince && ACT.UI[t].PageProvince !== -1 )
                {
                    h += "Province: <b>" + sender.find( 'select#Province:first option[value="' + ACT.UI[t].PageProvince + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ Province: <a style='color: #69f95a;'>" + sender.find( 'select#Province:first option[value="' + ACT.UI[t].PageProvince + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#Province' ).val( ACT.UI[t].PageProvince );
                }

                // Account
                if ( ACT.UI[t].PageAccount && ACT.UI[t].PageAccount !== -1 )
                {
                    h += "Account: <b>" + sender.find( 'select#Account:first option[value="' + ACT.UI[t].PageAccount + '"]' ).text() + "</b>~";
                    q += " <b class='italic'>[ Expense Type: <a style='color: #69f95a;'>" + sender.find( 'select#Account:first option[value="' + ACT.UI[t].PageAccount + '"]' ).text() + "</a> ]</b> ";

                    sender.find( 'select#Account' ).val( ACT.UI[t].PageAccount );
                }

                // Query
                if ( ACT.UI[t].PageQuery && ACT.UI[t].PageQuery !== "" )
                {
                    h += "Query: <b>" + ACT.UI[t].PageQuery + "</b>~";
                    q += " <b class='italic'>[ Query: <a style='color: #69f95a;'>" + ACT.UI[t].PageQuery + "</a> ]</b> ";

                    sender.find( 'input#Query' ).val( ACT.UI[t].PageQuery );
                }

                q += ". Refresh page to cancel...";

                if ( t !== "visual" && ( window.location.pathname !== "/Report" && window.location.hash !== "#detailed" ) )
                {
                    sender.find( ".custom-search-active-wrapper small" ).html( q );
                    sender.find( ".custom-search-active-wrapper" ).fadeIn( 1200 );
                }

                if ( $( "#s-header" ).length )
                {
                    $( "#s-header" ).html( h.split( '~' ).join( ' ' ) ).slideDown( 900 ).css( "display", "block" );
                }
            }

            // Check for any table List links that need appending the custom search
            if ( sender.find( 'a.append-search, a[data-append="1"]' ).length > 0 )
            {
                // Params
                var params = ACT.UI.GetCustomSearchParams( t );

                if ( sender.find( "th.sorting_asc" ).length )
                {
                    params.Sort = "ASC";
                    params.SortBy = sender.find( "th.sorting_asc:first" ).attr( "data-column" );
                }
                if ( sender.find( "th.sorting_desc" ).length )
                {
                    params.Sort = "DESC";
                    params.SortBy = sender.find( "th.sorting_desc:first" ).attr( "data-column" );
                }

                sender.find( 'a.append-search, a[data-append="1"]' ).each( function ()
                {
                    var hasQ = ( $( this ).attr( "href" ).indexOf( '?' ) > -1 );

                    var href = $( this ).attr( "href" ).split( '?' )[0] + "?IsCustomSearch=" + ics + "&type=" + $( this ).attr( "data-type" ) + "&" + $.param( params ).replace( /%5B%5D/g, '' );

                    $( this ).attr( "href", href );
                } );
            }
        },



        /** PLUGIN OVERRIDES **/

        DataValidateForm: function ( form )
        {
            var cntr = [];
            var valid = true;

            var direction = "center-left";
            var err = "<div class='message-error'>";

            var id = form.attr( "id" );

            // 1. Required field
            form.find( '[data-val-required], [required="1"], [required="required"]' ).each( function ()
            {
                if ( !$( this ).is( "select" ) && ( $( this ).is( ":hidden" ) || !$( this ).is( ":visible" ) ) )
                {
                    return;
                }

                if ( $( this ).val() === "" )
                {
                    if ( $( this ).is( "select" ) && $( this ).hasClass( "chzn" ) )
                    {
                        direction = "center-right";
                        cntr = form.find( 'div#s2id_' + $( this ).attr( "id" ) );
                    }
                    else
                    {
                        cntr = $( this );
                    }

                    err += "Please enter/select a value for this field to proceed!";

                    valid = false;

                    return false;
                }
            } );

            // 2. Valid File Types
            if ( valid && form.find( '[data-val-file="1"]' ).length && form.find( '[data-val-file="1"]' ).val() !== '' )
            {
                var val = form.find( '[data-val-file="1"]' ).val();
                var arr = val.split( '.' );
                var ext = arr[arr.length - 1];

                if ( $.inArray( ext.toLowerCase(), ACT.UI.DocumentTypes ) === -1 )
                {
                    cntr = form.find( '[data-val-file="1"]' );

                    err += "The file extension <b>" + ext + "</b> is not allowed! Allowed formats: " + ACT.UI.DocumentTypes.join( ',' );

                    valid = false;
                }
            }

            // 3. Service Type
            if ( valid && form.find( '[data-service-type="1"]' ).length && !form.find( '[data-service-type="1"]:checked' ).length )
            {
                valid = false;

                direction = "bottom-left";
                cntr = form.find( '[data-service-type="1"]:first' );

                err += "Please select a Service Type from here that you would like to register for.";
            }

            // 4. Estimated Load per Month
            if ( valid && form.find( "#EstimatedLoad-table" ).length )
            {
                form.find( "#EstimatedLoad-table" ).find( 'input[name]' ).each( function ()
                {
                    if ( $( this ).val() === "" )
                    {
                        valid = false;

                        direction = "center-right";
                        cntr = $( this );

                        err += "Enter your estimate for " + $( this ).attr( "name" ).replace( "EstimatedLoad.", "" );

                        return false;
                    }
                } );
            }

            // 5. User Accepted Ts & Cs
            if ( valid && form.find( '[name="IsAccpetedTC"]' ).length && !form.find( '[name="IsAccpetedTC"]:checked' ).length )
            {
                valid = false;

                direction = "bottom-left";
                cntr = form.find( '[name="IsAccpetedTC"]' );

                err += "One last critical thing, please click here to Accept our Terms & Conditions for signing up with <b>ACT Pallet Solutions</b>.";
            }

            // 6. Validate Approve/Decline PSP Status is selected?
            if ( valid && form.find( '#ApproveDeclinePSPClientStatus' ).length && form.find( '#ApproveDeclinePSPClientStatus' ).val() === "" )
            {
                valid = false;

                direction = "center-left";
                cntr = form.find( 'div#s2id_ApproveDeclinePSPClientStatus' );

                err += "Please select a Status for this PSP/Client application to utilize ACT Pallet Solutions.";
            }

            // 7. Validate Approve/Decline PSP Status is selected?
            if ( valid && form.find( '#ApproveDeclinePSPClientStatus' ).length && form.find( '#DeclineReason' ).length && form.find( '#ApproveDeclinePSPClientStatus' ).val() === "2" && form.find( '#DeclineReason' ).val() === "" )
            {
                valid = false;

                direction = "center-right";
                cntr = form.find( 'div#s2id_DeclineReason' );

                err += "Please select a reason to why you're declining this PSP/Client application to utilize ACT Pallet Solutions.";
            }

            // 7. Validate all user login details are supplied if PSP/Client is verified
            if ( valid && form.find( '#ApproveDeclinePSPClientStatus' ).length && form.find( '#user-details' ).length && form.find( '#ApproveDeclinePSPClientStatus' ).val() === "1" )
            {
                form.find( '#user-details input:visible' ).each( function ()
                {
                    if ( $( this ).val() === "" )
                    {
                        valid = false;

                        direction = "center-right";
                        cntr = $( this );

                        err += "Please enter/select a value for this field to proceed!";

                        return false;
                    }
                } );
            }

            // 8. Password match ConfirmPassword
            if ( valid && form.find( '#Password' ).length && form.find( '#ConfirmPassword' ).length && form.find( '#Password' ).val() !== form.find( '#ConfirmPassword' ).val() )
            {
                valid = false;

                direction = "center-right";
                cntr = form.find( '#ConfirmPassword' );

                err += "Password does not match";
            }

            // 8. Password match ConfirmPassword
            if ( valid && form.find( '#User_Password' ).length && form.find( '#User_ConfirmPassword' ).length && form.find( '#User_Password' ).val() !== form.find( '#User_ConfirmPassword' ).val() )
            {
                valid = false;

                direction = "center-right";
                cntr = form.find( '#User_ConfirmPassword' );

                err += "Password does not match";
            }


            if ( !valid )
            {
                err += "</div>";

                ACT.Sticky.StickyOne.addClass( "error" );
                ACT.Sticky.StickyOne.css( { "display": "none" } );

                ACT.Sticky.Show( cntr, "Error Submitting Your Form!", err, [], direction );
                $( "html, body" ).animate( { scrollTop: cntr.offset().top - 150 }, "slow", function () { cntr.focus(); } );
            }

            form.find( "#doc-uploads .doc-group" ).each( function ( i )
            {
                ACT.UI.DataIndex( $( this ).find( "input" ), i );
            } );

            return valid;
        },

        DataValidateNumber: function ( form )
        {
            var valid = true;

            var cntr = [];
            var direction = "center-left";
            var err = "<div class='message-error'>This is not a valid amount for this field.</div>";

            form.find( "input[data-val-number]" ).each( function ()
            {
                var i = $( this );

                if ( valid && !parseFloat( i.val().trim() ) )
                {
                    valid = false;
                    cntr = i;
                }
            } );

            if ( !valid )
            {
                ACT.Sticky.StickyOne.addClass( "error" );
                ACT.Sticky.StickyOne.css( { "display": "none" } );

                ACT.Sticky.Show( cntr, "Error Adding Product Option!", err, [], direction );
                $( "html, body" ).animate( { scrollTop: cntr.offset().top - 150 }, "slow", function () { cntr.focus(); } );

                return;
            }

            return valid;
        },

        DataDoSelected: function ( sel, options )
        {
            if ( typeof ( sel.attr( 'original-val' ) ) === 'undefined' )
            {
                sel.attr( 'original-val', sel.val() );
            }

            for ( var o in options )
            {
                if ( o.trim() === sel.attr( 'original-val' ).trim() ) return o;
            }

            return '';
        },

        DataIndex: function ( sender, index )
        {
            sender.each( function ( i )
            {
                i = ( typeof ( index ) !== 'undefined' ) ? index : i;

                if ( typeof ( $( this ).attr( "name" ) ) !== 'undefined' && $( this ).attr( "name" ).indexOf( "[" ) > 0 )
                {
                    var name1 = $( this ).attr( "name" );
                    var n1 = name1.split( "[" )[1].split( "]" )[0];

                    name1 = name1.replace( '[' + n1 + ']', '[' + i + ']' );

                    $( this ).attr( "name", name1 );
                }

                if ( typeof ( $( this ).attr( "id" ) ) !== 'undefined' && $( this ).attr( "id" ).indexOf( "[" ) > 0 )
                {
                    var name2 = $( this ).attr( "id" );
                    var n2 = name2.split( "[" )[1].split( "]" )[0];

                    name2 = name2.replace( '[' + n2 + ']', '[' + i + ']' );

                    $( this ).attr( "id", name2 );
                }

                if ( typeof ( $( this ).attr( "for" ) ) !== 'undefined' && $( this ).attr( "for" ).indexOf( "[" ) > 0 )
                {
                    var name3 = $( this ).attr( "for" );
                    var n3 = name3.split( "[" )[1].split( "]" )[0];

                    name3 = name3.replace( '[' + n3 + ']', '[' + i + ']' );

                    $( this ).attr( "for", name3 );
                }

                if ( $( this ).is( "a" ) && typeof ( $( this ).attr( "data-del-one-more" ) ) !== 'undefined' && typeof ( $( this ).attr( "data-target" ) ) !== 'undefined' )
                {
                    var target = $( this ).attr( "data-target" );
                    var t1 = target.split( "_" )[1].split( "_" )[0];

                    target = target.replace( '_' + t1 + '_', '_' + i + '_' );

                    $( this ).attr( "data-target", target );
                }

                if ( $( this ).is( "div" ) && typeof ( $( this ).attr( "id" ) ) !== 'undefined' && typeof ( $( this ).attr( "data-del-holder" ) ) !== 'undefined' )
                {
                    var id = $( this ).attr( "id" );
                    var t2 = id.split( "_" )[1].split( "_" )[0];

                    id = id.replace( '_' + t2 + '_', '_' + i + '_' );

                    $( this ).attr( "id", id );
                }
            } );

            return true;
        },

        DataShowSelected: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );
                var target = $( i.attr( "data-target" ) );

                i
                    .unbind( "change" )
                    .bind( "change", function ()
                    {
                        if ( $( this ).is( ":checked" ) )
                        {
                            target.find( 'tbody tr' ).each( function ()
                            {
                                var hasChk = $( this ).find( '[type="checkbox"]' ).length;

                                if ( !hasChk || ( hasChk && !$( this ).find( '[type="checkbox"]:checked' ).length ) )
                                {
                                    $( this )
                                        .add( target.find( '#page-count' ) )
                                        .add( target.find( '#page-navigation' ) )
                                        .slideUp( 500 );
                                }
                            } );
                        }
                        else
                        {
                            target.find( 'tbody tr' ).each( function ()
                            {
                                var hasChk = $( this ).find( '[type="checkbox"]' ).length;

                                if ( !hasChk || ( hasChk && !$( this ).find( '[type="checkbox"]:checked' ).length ) )
                                {
                                    $( this )
                                        .add( target.find( '#page-count' ) )
                                        .add( target.find( '#page-navigation' ) )
                                        .slideDown( 500 );
                                }
                            } );
                        }
                    } );
            } );
        },

        DataBank: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );
                var target = $( i.attr( "data-target" ) );

                i
                    .unbind( "change" )
                    .bind( "change", function ()
                    {
                        var d = { bankId: $( this ).val() };

                        if ( $( this ).val() === "" ) 
                        {
                            target.val( '' ).removeAttr( "readonly" );

                            return;
                        }

                        $.ajax( {
                            url: siteurl + "/GetBank",
                            type: "POST",
                            data: JSON.stringify( d ),
                            contentType: "application/json; charset=utf-8",
                            dataType: "json",
                            error: function ( e )
                            {

                            },
                            success: function ( s )
                            {
                                target.val( s.Code.trim() ).change();

                                if ( s.Code.trim() !== '' )
                                {
                                    target.attr( "readonly", "readonly" );
                                }
                                else
                                {
                                    target.removeAttr( "readonly" );
                                }

                                ACT.UI.DataHighlightFields( target.parent() );
                            }
                        } );
                    } );
            } );
        },

        DataValMax: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                if ( i.is( ":hidden" ) )
                {
                    return;
                }

                var id = i.attr( "id" );
                var max = parseInt( i.attr( "data-val-length-max" ) );

                var papa = i.parent();
                var target = $( '[for="' + id + '"]' );
                var counter = '<em class="slick-counter" id="counting-' + id + '"></em>';

                if ( !target.length && !papa.find( "#counting-" + id ).length )
                {
                    papa.append( '<label for="' + id + '"></label>' );
                }
                else if ( !papa.find( "#counting-" + id ).length )
                {
                    target.css( { "width": "90%" } ).append( counter );
                }

                i
                    .bind( "keypress", function ( e )
                    {
                        e = ( e ) ? e : window.event;

                        var charCode = ( e.which ) ? e.which : e.keyCode;

                        if ( charCode === 8 || charCode === 46 || charCode === 35 || charCode === 36 || charCode === 37 || charCode === 39 )
                        {
                            return true;
                        }

                        if ( ( $( this ).val().length + 1 ) > max )
                        {
                            $( "#counting-" + id ).css( 'display', 'none' );

                            return false;
                        }
                    } )
                    .bind( "keyup change", function ()
                    {
                        if ( $( this ).val().length <= 0 )
                        {
                            $( "#counting-" + id ).fadeOut( 900 );

                            return;
                        }

                        $( "#counting-" + id ).text( $( this ).val().length + " of " + max + " characters" ).fadeIn( 1200 );
                    } )
                    .bind( "blur", function ()
                    {
                        var val = $( this ).val().substr( 0, max );

                        $( this )
                            .val( val )
                            .attr( "value", val );
                    } );
            } );
        },

        DataBankValidation: function ( sender )
        {
            function ValidateBank( sender )
            {
                var papa = sender.parent().parent();

                var acc = papa.find( '[data-name="AccountNo"]' );
                var bcode = papa.find( '[data-name="BranchCode"]' );
                var accType = papa.find( '[data-name="AccountType"]' );

                if ( acc.val() === "" || bcode.val() === "" || accType.val() === "" ) return;

                title = "Bank Account Details Validation";
                msg = "<p>";
                msg += " Please wait whilst we validate the provided Bank Account details...";
                msg += ' <img id="loader" class="apcloud-loader" src="' + imgurl + '/images/loader.gif" alt="" style="margin: 0 5px;" />';
                msg += "</p>";

                var d = { accountNo: acc.val(), branchCode: bcode.val(), accountType: accType.val() };

                $( 'html, body' ).css( { 'cursor': 'progress' } );
                ACT.Modal.Open( msg, title, false );

                $.ajax( {
                    url: siteurl + "/IsValidBankDetails",
                    type: "POST",
                    data: JSON.stringify( d ),
                    contentType: "application/json; charset=utf-8",
                    dataType: "json",
                    error: function ( e )
                    {

                    },
                    success: function ( s )
                    {
                        ACT.Loader.Hide();

                        if ( s.Code === '0' )
                        {
                            ACT.Modal.Close();
                            $( "form:visible" ).find( "#save-btn, #sdoc-btn" ).removeAttr( "title" ).removeAttr( "disabled" );

                            acc.addClass( "b-valid" );
                            bcode.addClass( "b-valid" );
                            $( "div#s2id_" + accType.attr( "id" ) ).addClass( "b-valid" );
                        }
                        else
                        {
                            $( "form:visible" ).find( "#save-btn, #sdoc-btn" ).attr( { "disabled": "disabled", "title": "Can't submit form: Bank validation failed." } );

                            msg = "<div class='message-error'>" + s.Message + "</div>";
                            ACT.Modal.Open( msg, title, false );

                            acc.removeClass( "b-valid" );
                            bcode.removeClass( "b-valid" );
                            $( "div#s2id_" + accType.attr( "id" ) ).removeClass( "b-valid" );
                        }
                    }
                } );
            }

            sender
                .unbind( "change" )
                .bind( "change", function ()
                {
                    var i = $( this );

                    clearTimeout( ACT.UI.PageSearchTimer );

                    ACT.UI.PageSearchTimer = setTimeout( function ()
                    {
                        ValidateBank( i );
                    }, '1000' );
                } );
        },

        DataGetBroadcast: function ()
        {
            if ( ACT.UI.PageBroadcast ) return;

            ACT.UI.PageBroadcast = 1;

            $.ajax( {
                url: siteurl + "/GetBroadcast",
                type: "POST",
                data: {},
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                error: function ( e )
                {

                },
                success: function ( b )
                {
                    if ( b.Found === 1 )
                    {
                        var msg = '<p>' + b.Message.replace( /\r\n/g, '<br />' ) + '</p>';
                        msg += '<p>';
                        msg += '    <input id="btn-got-it" type="button" class="btn-yes" value="I read this message, do not display it again" />';
                        msg += '</p>';

                        setTimeout( function ()
                        {
                            ACT.Modal.Open( msg, 'Attention', false );
                            $( ".announcement" ).slideDown( 1200 );

                            var btn = $( ACT.Modal.Container ).find( '#modal-body #btn-got-it' );
                            btn
                                .unbind( "click" )
                                .bind( "click", function ()
                                {
                                    AddUserBroadcast( btn, b.Bid );
                                } );
                        }, '4000' );
                    }
                }
            } );

            function AddUserBroadcast( sender, bid )
            {
                ACT.UI.Post( sender, $( "#empty-div" ), siteurl + '/AddUserBroadcast', { id: bid }, [], true, true );

                $( ".announcement" ).hide( 500 );
                ACT.Modal.Close();
            }
        },

        DataMoney: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                i.bind( "blur", function ()
                {
                    var f = $( this ).val().split( /\s+/ ).join( '' ).replace( 'R', '' ).replace( /,/g, '.' );

                    if ( !parseFloat( f ) )
                    {
                        return;
                    }

                    $( this ).val( "R" + parseFloat( f ).money( 2 ) );
                } );
            } );
        },

        DataCheckAll: function ( target )
        {
            target
                .find( 'input[data-all="1"]' )
                .unbind( "change" )
                .bind( "change", function ()
                {
                    var kids = target.find( 'input[data-child="1"]' );

                    if ( $( this ).is( ":checked" ) )
                    {
                        kids.each( function ()
                        {
                            if ( typeof ( $( this ).attr( "disabled" ) ) !== 'undefined' ) return;

                            $( this ).prop( "checked", true ).attr( "checked", "checked" );
                        } );
                    }
                    else
                    {
                        kids.prop( "checked", false ).removeAttr( "checked" );
                    }
                } );
        },

        DataCallBack: function ( callback )
        {
            if ( typeof ( callback ) === 'undefined' )
            {
                return;
            }

            if ( typeof ( callback ) === typeof ( Function ) )
            {
                try
                {
                    callback();
                }
                catch ( e )
                {
                    eval( callback );
                }
            }
            else
            {
                eval( callback );
            }
        },

        DataDOB: function ( sender )
        {
            var dob = $( "#DateOfBirth" );

            sender.each( function ()
            {
                var i = $( this );

                i
                    .unbind( "change" )
                    .bind( "change", function ()
                    {
                        var y = $( '[data-name="year"]' ).val();
                        var m = $( '[data-name="month"]' ).val();
                        var d = $( '[data-name="day"]' ).val();

                        dob.val( y + "/" + m + "/" + d );
                    } );
            } );
        },

        DataEmailFile: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var id = i.attr( "data-id" );
                var subj = i.attr( "data-subject" );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        var title = i.attr( "original-title" );
                        var message = '<p style="margin-top: 10px;">Please enter an e-mail address below to continue.</p>';

                        message += '<p>';
                        message += '  Email Address:<br />';
                        message += '  <input id="email" name="email" value="" type="text" placeholder="Enter E-mail Address" />';
                        message += '</p>';

                        message += '<p>';
                        message += '  Subject of the Email (Optional):<br />';
                        message += '  <textarea id="subject" name="subject" readonly="readonly" placeholder="Enter Email Subject (Optional)" style="width: 95%; height: auto;">' + subj + '</textarea>';
                        message += '</p>';

                        message += '<p style="padding-bottom: 5px; border-bottom: 1px dashed #ddd;">';
                        message += '  Message for the Email Recipient (Optional):<br />';
                        message += '  <textarea id="message" name="message" placeholder="Enter Message for the Email Recipient (Optional)" style="width: 95%;"></textarea>';
                        message += '</p>';

                        message += '<p>';
                        message += '  <input id="yes-btn" value="Send" type="button" class="btn-yes" />';
                        message += '  <span style="padding: 0 5px;">/</span>';
                        message += '  <input id="no-btn" value="Cancel" type="button" class="btn-no" />';
                        message += '</p>';

                        ACT.Sticky.Show( i, title, message, [], "center-right" );

                        var no = ACT.Sticky.StickyOne.find( "#no-btn" );
                        var yes = ACT.Sticky.StickyOne.find( "#yes-btn" );

                        no
                            .unbind( "click" )
                            .bind( "click", function ()
                            {
                                ACT.Sticky.Hide();
                            } );

                        yes
                            .unbind( "click" )
                            .bind( "click", function ()
                            {
                                var valid = true;

                                var email = ACT.Sticky.StickyOne.find( "#email" );
                                var subject = ACT.Sticky.StickyOne.find( "#subject" );
                                var message = ACT.Sticky.StickyOne.find( "#message" );

                                if ( email.val().trim() === '' )
                                {
                                    valid = false;
                                    email.addClass( 'invalid' ).focus();
                                }

                                if ( !valid )
                                {
                                    return false;
                                }

                                ACT.Loader.Show( yes, true );

                                $.post( "/Financials/EmailStatement", { id: id, email: email.val(), subject: subject.val(), message: message.val() }, function ( data )
                                {
                                    var d = $( "<div/>" ).html( data );

                                    ACT.Loader.Hide();
                                    ACT.Sticky.Show( i, d.find( ".title" ).text(), d.find( ".message" ).html(), [], "center-right" );
                                } );
                            } );

                        return false;
                    } );
            } );
        },

        DataServiceType: function ( sender )
        {
            var cTnC = $( "#ClientTnCDocumentUrl" ).val(),
                pTnC = $( "#PlatformTnCDocumentUrl" ).val();

            var tncLink = $( "#tnc-link" );

            sender.each( function ()
            {
                var i = $( this );

                var v = i.val();
                var t = $( i.attr( "data-target" ) );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        if ( v === "1" || v === "2" )
                        {
                            t.show( 900 );

                            tncLink.attr( "href", cTnC );
                        }
                        else
                        {
                            t.hide( 900 );

                            tncLink.attr( "href", pTnC );
                        }
                    } );
            } );
        },

        DataAuditLog: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var id = i.attr( "data-pid" );
                var type = i.attr( "data-type" );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        ACT.Loader.Show( i.find( "span" ), true );

                        $( "<div />" ).load( siteurl + "/PopAuditLog", { id: id, type: type }, function ( data )
                        {
                            ACT.Loader.Hide();

                            $( ".modalBody" ).css( { "color": "#000" } );
                            $( ".modalContent" ).css( { "min-width": "1000px" } );

                            $( ".tipsy" ).remove();

                            ACT.Modal.Open( data, type + " Audit Log", false, [] );

                            setTimeout( function ()
                            {
                                ACT.UI.DataDetails( $( '*[data-details="1"]' ) );
                            }, "1000" );
                        } );
                    } );
            } );
        },

        DataApproveDeclinePSP: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var activeTarget = $( i.attr( "data-active-target" ) );
                var rejectedTarget = $( i.attr( "data-rejected-target" ) );

                i
                    .unbind( "change" )
                    .bind( "change", function ()
                    {
                        if ( $( this ).val() === "1" )
                        {
                            //
                            rejectedTarget.css( "display", "none" );
                            activeTarget.show( 900 );
                        }
                        else if ( $( this ).val() === "2" )
                        {
                            //
                            activeTarget.css( "display", "none" );
                            rejectedTarget.show( 900 );
                        }
                        else if ( $( this ).val() === "" )
                        {
                            //
                            activeTarget.hide( 900 );
                            rejectedTarget.hide( 900 );
                        }
                    } );
            } );
        },

        DataRole: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var pspOptions = $( i.attr( "data-psp" ) );
                var clientOptions = $( i.attr( "data-client" ) );

                var pspRoleId = i.attr( "data-psp-role-id" );
                var clienntRoleId = i.attr( "data-client-role-id" );

                i
                    .unbind( "change" )
                    .bind( "change", function ()
                    {
                        if ( pspRoleId === $( this ).val() )
                        {
                            clientOptions.css( "display", "none" );
                            pspOptions.show( 1000 );
                        }
                        else if ( clienntRoleId === $( this ).val() )
                        {
                            pspOptions.css( "display", "none" );
                            clientOptions.show( 1000 );
                        }
                        else
                        {
                            pspOptions.hide( 1000 );
                            clientOptions.hide( 1000 );
                        }
                    } );
            } );
        },

        DataGraphs: function ( sender, params )
        {
            var i = sender.parent().find( 'div[data-loaded="0"]' ).first();

            if ( !i.length )
            {
                setTimeout( function ()
                {
                    $( ".gs-data" ).show( 1000 );
                }, 4000 );

                return;
            }

            i.html( "" );
            i.append( "<div id='loader' />" );

            function LoadNext()
            {
                i.attr( "data-loaded", 1 );

                ACT.UI.DataGraphs( sender, params );
            }

            if ( typeof ( params ) === 'undefined' )
            {
                params = {};
            }

            ACT.UI.Get( i.find( "#loader" ), i, siteurl + "/" + i.attr( "data-type" ), params, LoadNext(), true, true );
        },

        AgeOfOutstandingPallets: function ( sender, months )
        {
            Highcharts.chart( sender, {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false,
                    type: 'pie'
                },
                title: {
                    text: 'Age Of Outstanding Pallets'
                },
                tooltip: {
                    pointFormat: '{series.name}: <b>{point.y:.0f}</b>'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '<b style="color:{point.color};">{point.name}</b>: <span style="color:{point.color};">{point.y:.0f}</span>'
                        }
                    }
                },
                series: [{
                    name: 'NO. OF OUTSTANDING PALLETS',
                    colorByPoint: true,
                    data: months
                }]
            } );
        },

        LoadsPerMonth: function ( sender, months, series )
        {
            Highcharts.chart( sender, {
                title: {
                    text: 'Loads Per Month'
                },
                xAxis: {
                    categories: months
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Total'
                    },
                    stackLabels: {
                        enabled: true
                    }
                },
                legend: {
                    x: 0,
                    y: 5,
                    align: 'center',
                    verticalAlign: 'bottom'
                },
                plotOptions: {
                    column: {
                        stacking: 'normal',
                        dataLabels: {
                            enabled: true
                        }
                    }
                },
                series: series
            } );
        },

        AuthorisationCodesPerMonth: function ( sender, months, series )
        {
            Highcharts.chart( sender, {
                chart: {
                    marginBottom: 70
                },
                title: {
                    text: 'Authorisations Per Month'
                },
                xAxis: {
                    categories: months
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Total'
                    },
                    stackLabels: {
                        enabled: true
                    }
                },
                legend: {
                    x: 0,
                    y: 5,
                    align: 'center',
                    verticalAlign: 'bottom'
                },
                tooltip: {
                    headerFormat: '<b>{point.x}</b><br/>',
                    pointFormat: '{series.name}: {point.y}<br/>Total: {point.stackTotal}'
                },
                plotOptions: {
                    column: {
                        stacking: 'normal',
                        dataLabels: {
                            enabled: true
                        }
                    }
                },
                series: series
            } );
        },

        NumberOfPalletsManaged: function ( sender, months, series )
        {
            Highcharts.chart( sender, {
                chart: {
                    type: 'line'
                },
                title: {
                    text: 'Number Of Pallets Managed'
                },
                xAxis: {
                    categories: months
                },
                yAxis: {
                    title: {
                        text: 'Pallets'
                    }
                },
                plotOptions: {
                    line: {
                        dataLabels: {
                            enabled: true
                        }
                    },
                    series: {
                        label: {
                            connectorAllowed: false
                        }
                    }
                },
                series: series,

                responsive: {
                    rules: [{
                        chartOptions: {
                            legend: {
                                layout: 'horizontal',
                                align: 'center',
                                verticalAlign: 'bottom'
                            }
                        }
                    }]
                }
            } );
        },

        NumberOfDisputes: function ( sender, months, series )
        {
            Highcharts.chart( sender, {
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Number Of Disputes'
                },
                xAxis: {
                    categories: months
                },
                yAxis: {
                    title: {
                        text: 'Disputes'
                    }
                },
                plotOptions: {
                    line: {
                        dataLabels: {
                            enabled: true
                        }
                    }
                },
                series: series
            } );
        },

        DataGSRegion: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                i
                    .unbind( "change" )
                    .bind( "change", function ()
                    {
                        ACT.Loader.Show( $( 'label[for="SiteId"]' ), true );

                        $( "select#SiteId" ).parent().load( siteurl + "GetHtmlSiteList", { regionId: $( this ).val() }, function ()
                        {
                            ACT.UI.DataHighlightFields( $( "select#SiteId" ).parent() );

                            ACT.UI.DataGSSite( $( '*[data-gs-site="1"]' ) );

                            $( "select#SiteId" ).change();

                            ACT.Init.PluginInit( $( "select#SiteId" ).parent() );
                        } );
                    } );
            } );
        },

        DataGSSite: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                i
                    .unbind( "change" )
                    .bind( "change", function ()
                    {
                        var siteIds = "";

                        // Only set siteIds if no site is selected..

                        if ( $( this ).val() === "" )
                        {
                            $( this ).find( "option" ).each( function ( x )
                            {
                                if ( $( this ).attr( "value" ) === "" ) return;

                                if ( x > 0 )
                                {
                                    siteIds += "&";
                                }

                                siteIds += "siteIds=" + $( this ).attr( "value" );
                            } );
                        }

                        $( "select#ClientId" ).parent().load( siteurl + "GetHtmlClientList?" + siteIds, { siteId: $( this ).val() }, function ()
                        {
                            ACT.Loader.Hide();

                            ACT.UI.DataHighlightFields( $( "select#ClientId" ).parent() );

                            ACT.Init.PluginInit( $( "select#ClientId" ).parent() );
                        } );
                    } );
            } );
        },

        DataGSearch: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var filters = $( i.attr( "data-filters" ) );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        // These params should match the ACT.Core/Models/CustomSearchModel.cs

                        var params = ACT.UI.GetDashSearchParams( filters );

                        $( ".gs-data" ).css( "display", "none" );
                        $( '*[data-graph="1"]' ).attr( "data-loaded", 0 );

                        ACT.UI.DataGraphs( $( '*[data-graph="1"]' ), params );
                    } );
            } );
        },

        GetDashSearchParams: function ( filters )
        {
            return {
                GiveData: false,
                SiteId: filters.find( "#SiteId" ).val(),
                ClientId: filters.find( "#ClientId" ).val(),
                RegionId: filters.find( "#RegionId" ).val(),
                ToDate: filters.find( '[name="ToDate"]' ).val(),
                FromDate: filters.find( '[name="FromDate"]' ).val()
            };
        },

        DataGSData: function ( sender )
        {
            sender.each( function ()
            {
                var i = $( this );

                var type = i.attr( "data-type" );
                var arrow = i.attr( "data-arrow" );
                var target = $( i.attr( "data-target" ) );

                i
                    .unbind( "click" )
                    .bind( "click", function ()
                    {
                        var loaded = i.attr( "data-loaded" );

                        if ( loaded === "1" )
                        {
                            $( ".sticky-frame" ).css( { "width": ( $( "#item-list" ).outerWidth() - 60 ), "max-width": ( $( "#item-list" ).outerWidth() - 60 ) } );

                            ACT.Sticky.Show( i, "Data", target.html(), [], arrow );

                            return;
                        }

                        var params = ACT.UI.GetDashSearchParams( $( "#gs-search-fields" ) );

                        params.GiveData = true;

                        ACT.Loader.Show( i.find( "i" ) );

                        target.load( siteurl + "/" + type, params, function ( data )
                        {
                            $( ".sticky-frame" ).css( { "width": ( $( "#item-list" ).outerWidth() - 60 ), "max-width": ( $( "#item-list" ).outerWidth() - 60 ) } );

                            ACT.Sticky.Show( i, "Data", data, ACT.Loader.Hide(), arrow );

                            i.attr( "data-loaded", "1" );

                            //ACT.Loader.Hide();
                        } );
                    } );
            } );
        }
    };
} )();
