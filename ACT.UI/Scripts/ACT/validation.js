(function ()
{
    ACT.Validation = {

        Email: /^[\w-]+(\.[\w-]+)*@([\w-]+\.)+[a-zA-Z]{2,7}$/,

        Int: /(^[-]?[1-9]\d+$)|(^[-]?[1-9]$)|(^0$)|(^[-]?[1-9]\d+\.\d$)|(^[-]?[0-9]\.\d$)/,

        IdNumber: /^(((\d{2}((0[13578]|1[02])(0[1-9]|[12]\d|3[01])|(0[13456789]|1[012])(0[1-9]|[12]\d|30)|02(0[1-9]|1\d|2[0-8])))|([02468][048]|[13579][26])0229))(( |-)(\d{4})( |-)(\d{3})|(\d{7}))$/,

        Start: function ()
        {
            $('.pr-validation').unbind('click');
            $('.pr-validation').click(function ()
            {
                ACT.Validation.Validate($(this));
            });

            $('*[data-id-number="1"]').each(function ()
            {
                var i = $(this);

                i
                .bind("blur change", function ()
                {
                    var errCntr = $('[data-valmsg-for="' + $(this).attr("id") + '"]');

                    if ($(this).val() != "" && !ACT.Validation.IdNumber.test($(this).val()))
                    {
                        errCntr
                        .removeClass("field-validation-valid")
                        .addClass("field-validation-error")
                        .html('<span for="' + $(this).attr("id") + '" generated="true" class="">' + $(this).attr("data-error") + '</span>');

                        $("#save-btn").attr({ "disabled": "disabled", "title": $(this).attr("data-error") });
                    }
                    else
                    {
                        errCntr
                        .removeClass("field-validation-error")
                        .addClass("field-validation-valid");

                        $("#save-btn").removeAttr("title").removeAttr("disabled");
                    }
                });
            });
        },

        Validate: function (sender)
        {
            var valid = true;
            var nobg = sender.attr('no-bg');
            var successFunction = sender.attr('success-function');

            $('[group-name="' + sender.attr('target-group') + '"]').each(function (i)
            {
                var val = $(this).val();
                var mask = $(this).attr('mask');
                var min = $(this).attr('data-min');
                var max = $(this).attr('data-max');
                var compareWith = $(this).attr('compare-with');
                var allowedFormat = $(this).attr('allowed-format');
                var inputRequired = $(this).attr('input-required');
                var req = inputRequired;

                if (mask)
                {
                    mask = (inputRequired && inputRequired == '1' && val != mask) ? true : (inputRequired && inputRequired == '0') ? true : (!inputRequired) ? true : false;
                }
                else
                {
                    mask = true;
                }

                if (inputRequired)
                {
                    inputRequired = (inputRequired == '1' && $(this).val() != '') ? true : (inputRequired == '0') ? true : false;
                }
                else
                {
                    inputRequired = true;
                }

                if ((allowedFormat && req && req != '0') || (allowedFormat && req == '1'))
                {
                    if (allowedFormat == 'email')
                        allowedFormat = ACT.Validation.Email.test(val);
                    else if (allowedFormat == 'int')
                        allowedFormat = ACT.Validation.Int.test(val);
                }
                else
                {
                    allowedFormat = true;
                }

                if ((compareWith && req && req != '0') || (compareWith && $(compareWith).val() != $(compareWith).attr('mask')))
                {
                    var compareWithVal = $(compareWith).val();
                    compareWith = compareWithVal == val;
                }
                else
                {
                    compareWith = true;
                }

                if (min && val.length < parseInt(min))
                {
                    min = false;
                }
                else
                {
                    min = true;
                }

                if (max && val.length > parseInt(max))
                {
                    max = false;
                }
                else
                {
                    max = true;
                }

                if (!allowedFormat || !compareWith || !inputRequired || !mask || !min || !max)
                {
                    valid = valid && false;

                    if (nobg && nobg == 'true')
                        $(this).css({ 'color': 'red' });
                    else
                        $(this).addClass('invalid');
                }
                else
                {
                    valid = valid && true;

                    $(this).removeClass('invalid');
                }
            });

            if (valid)
            {
                if (successFunction != undefined)
                    eval(successFunction);
            }
            else
            {
                var top = $('[group-name="' + sender.attr('target-group') + '"].invalid:first');
                top = (top && top.length > 0) ? top.offset().top - 60 : 0;
                $('html, body').animate({ scrollTop: top }, 'slow', function () { });
                $('[group-name="' + sender.attr('target-group') + '"].invalid:first').focus();
            }

            return valid;
        }
    }
})();