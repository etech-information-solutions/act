jQuery.fn.center = function ()
{
    this.css( "position", "absolute" );

    this.css( "top", Math.max( 0, ( ( $( window ).height() - $( this ).outerHeight() ) / 2 ) + $( window ).scrollTop() ) + 100 + "px" );
    this.css( "left", Math.max( 0, ( ( $( window ).width() - $( this ).outerWidth() ) / 2 ) + $( window ).scrollLeft() ) + "px" );

    return this;
}

String.prototype.equals = function ( s )
{
    return ( this == s );
};

String.prototype.equalsIgnoreCase = function ( s )
{
    return ( this.toLowerCase() === s.toLowerCase() );
};

Number.prototype.money = function ( c, d, t )
{
    var n = this,
        c = isNaN( c = Math.abs( c ) ) ? 2 : c,
        d = typeof ( d ) === "undefined" ? "," : d,
        t = typeof ( t ) === "undefined" ? " " : t,
        s = n < 0 ? "-" : "",
        i = parseInt( n = Math.abs( +n || 0 ).toFixed( c ) ) + "",
        j = ( j = i.length ) > 3 ? j % 3 : 0;

    return s + ( j ? i.substr( 0, j ) + t : "" ) + i.substr( j ).replace( /(\d{3})(?=\d)/g, "$1" + t ) + ( c ? d + Math.abs( n - i ).toFixed( c ).slice( 2 ) : "" );
};



jQuery.fn.prTop = function ( top )
{
    if ( top )
    {
        $( this ).css( "top", top );

        return $( this );
    }

    return $( this ).offset().top;
}

jQuery.fn.prLeft = function ( left )
{
    if ( left )
    {
        $( this ).css( "left", left );

        return $( this );
    }

    return $( this ).offset().left;
}

jQuery.fn.prBottom = function ( bottom )
{
    if ( bottom )
    {
        $( this ).css( "bottom", bottom );

        return $( this );
    }

    return $( window ).outerHeight() - ( $( this ).prTop() + $( this ).outerHeight() );
}

jQuery.fn.prRight = function ( right )
{
    if ( right )
    {
        $( this ).css( "right", right );

        return $( this );
    }

    return $( window ).outerWidth() - ( $( this ).prLeft() + $( this ).outerWidth() );
}

jQuery.fn.chznreadonly = function ( readonly )
{
    var chzn = $( this ).parent().find( "div.chzn" );

    if ( !chzn.length ) return $( this );

    if ( readonly )
    {
        $( this ).attr( "readonly", "readonly" );

        chzn.prepend( '<div class="disabled-select"></div>' );
    }
    else
    {
        $( this ).removeAttr( "readonly" );

        chzn.find( ".disabled-select" ).remove();
    }

    return $( this );
}


toastr.options = {
    "closeButton": false,
    "debug": false,
    "newestOnTop": false,
    "progressBar": false,
    "positionClass": "toast-top-right",
    "preventDuplicates": false,
    "onclick": null,
    "showDuration": "300",
    "hideDuration": "1000",
    "timeOut": "5000",
    "extendedTimeOut": "1000",
    "showEasing": "swing",
    "hideEasing": "linear",
    "showMethod": "fadeIn",
    "hideMethod": "fadeOut"
}


var ACT = {
    UI: {},
    Init: {},
    Modal: {},
    Loader: {},
    Validation: {}
}

