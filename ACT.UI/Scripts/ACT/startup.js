


ACT.Init = {

    NotificationTimeout: [],

    Start: function (restartPlugin)
    {
        $(".tipsy").remove();
        ACT.Loader.Hide();

        $(".notification").stop().slideDown(700, function ()
        {
            $('html, body').animate({ scrollTop: $(this).prTop() - 20 }, 'slow', function () { });
        });

        clearTimeout(this.NotificationTimeout);
        this.NotificationTimeout = setTimeout(function ()
        {
            $(".notification").stop().animate({
                "width": "0",
                "height": "0",
                "opacity": "0",
                "margin-top": "-50px",
                "filter": "alpha(opacity=0)"
            }, 1200, function ()
            {
                $(this).remove();
            });

        }, "120000");

        if (!this.PluginLoaded || restartPlugin)
        {
            this.PluginInit();
        }

        this.PluginLoaded = true;

        ACT.UI.Start();
        ACT.Validation.Start();

        if ($("#edit-item").length)
        {
            this.AppendPaging($("#edit-item"));
        }
    },

    AppendPaging: function (sender, t)
    {
        if (!t)
        {
            t = $(".da-tab:visible").attr("id");
        }

        if (!ACT.UI[t])
        {
            ACT.UI[t] = [];
        }

        //var params = ACT.UI.GetCustomSearchParams(t);

        //for (p in params)
        //{
        //    if (!sender.find("#" + p).length)
        //    {
        //        sender.append('<input id="' + p + '" type="hidden" name="' + p + '" value="' + params[p] + '" />');
        //    }
        //}

        if (!$('input[name="skip"]').length)
        {
            sender.append('<input type="hidden" name="skip" value="' + (ACT.UI[t].PageSkip || ACT.UI.PageSkip) + '" />');
        }
        else
        {
            $('input[name="skip"]').val((ACT.UI[t].PageSkip || ACT.UI.PageSkip));
        }

        if (!$('input[name="page"]').length)
        {
            sender.append('<input type="hidden" name="page" value="' + (ACT.UI[t].PageNumber || ACT.UI.PageNumber) + '" />');
        }
        else
        {
            $('input[name="page"]').val((ACT.UI[t].PageNumber || ACT.UI.PageNumber));
        }

        if (!$('input[name="take"]').length)
        {
            sender.append('<input type="hidden" name="take" value="' + (ACT.UI[t].PageLength || ACT.UI.PageLength) + '" />');
        }
        else
        {
            $('input[name="take"]').val((ACT.UI[t].PageLength || ACT.UI.PageLength));
        }

        if (!$('input[name="query"]').length)
        {
            sender.append('<input type="hidden" name="query" value="' + (ACT.UI[t].PageSearch || ACT.UI.PageSearch) + '" />');
        }
        else
        {
            $('input[name="query"]').val((ACT.UI[t].PageSearch || ACT.UI.PageSearch));
        }
    },

    PluginInit: function (target)
    {
        target = target || $("body");

        // Tool tips
        target.find('a[rel="tipsy"], a[rel="tipsyS"]').tipsy({ fade: true, gravity: 's' });

        target.find('a[rel="tipsyN"]').tipsy({ fade: true, gravity: 'n' });

        target.find('a[rel="tipsyW"]').tipsy({ fade: true, gravity: 'w' });

        target.find('a[rel="tipsyE"]').tipsy({ fade: true, gravity: 'e' });


        // Date picker
        target.find('.timepicker, .time-picker').timepicker({ timeFormat: 'HH:mm:ss' });
        //target.find('.datepicker, .date-picker').datepicker({ inline: true, dateFormat: "dd-mm-yy" });
        target.find('.datetimepicker, .date-time-picker').datetimepicker({ ampm: true, dateFormat: "yy/mm/dd" });

        target.find('.datepicker, .date-picker').each(function (i)
        {
            var d = $(this);

            if (d.hasClass("hasDatepicker")) return;

            d.attr("id", d.attr("id") + "_" + i);

            var year = (new Date()).getFullYear();

            d.datepicker({
                inline: true,
                changeMonth: true,
                changeYear: true,
                dateFormat: "yy/mm/dd",
                yearRange: "1940:" + year,
                onChangeMonthYear: function ()
                {
                    ACT.Sticky.Close = false;

                    setTimeout("ACT.Sticky.Close = true;", "100");
                }
            });

            $(document).on('click', '#ui-datepicker-div', function ()
            {
                ACT.Sticky.Close = false;

                setTimeout("ACT.Sticky.Close = true;", "100");
            });
        });

        $.validator.methods.date = function (value, element)
        {
            return this.optional(element) || $.datepicker.parseDate('yy/mm/dd', value);
        };

        target.find("table.datatable-numberpaging").each(function ()
        {
            var i = $(this);

            if (i.hasClass("dataTable")) return true;

            if (i.find("tbody tr td").length > 1)
            {
                i.dataTable({
                    bSort: false,
                    bPaginate: false,
                    iDisplayLength: 50,
                    select: {
                        style: 'single'
                        //style: 'multi'
                    },
                    //action: function (e, dt, node, config) {
                    //    var data = oTable.rows({ selected: true }).data();
                    //    console.log("data---" + data);
                    //},
                    //"fixedHeader": {
                    //    header: i.hasClass( "fixed-table" )
                    //},
                    "fnDrawCallback": function ()
                    {
                        ACT.UI.Start();
                    }
                });
            }
        });

        target.find("table.da-table").each(function ()
        {
            var i = $(this);

            if (i.hasClass("dataTable")) return true;

            i.dataTable({
                bSort: false,
                bPaginate: false,
                iDisplayLength: 50,
                select: {
                    style: 'single'
                    //style: 'multi'
                },
                //action: function (e, dt, node, config) {
                //    var data = oTable.rows({ selected: true }).data();
                //    console.log("data---" + data);
                //},
                //"fixedHeader": {
                //    header: i.hasClass("fixed-table")
                //},
                "fnDrawCallback": function ()
                {
                    ACT.UI.Start();
                }
            });
        });

        if ($.fn.select2)
        {
            target.find("select.chzn").select2();
        }

        $('a[rel="fancybox"]').fancybox({
            'type': "image",
            'opacity': true,
            'overlayShow': true,
            'overlayColor': '#000',
            'transitionIn': 'fade',
            'transitionOut': 'fade',
            'overlayOpacity': '0.8',
            'titlePosition': 'inside'
        });
    }
};







$(function ()
{
    ACT.Init.Start(true);
});