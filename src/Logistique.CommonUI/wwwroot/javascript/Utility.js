window.JsonStringify = function (obj) {
    var str = JSON.stringify(JSON.parse(obj), null, 6);
    return str;
};
window.JsonParse = function (str) {
    var obj = JSON.parse(str);
    return obj.toString();
};
window.addEventListener('resize', () => {
    // We execute the same script as before
    let vh = window.innerHeight * 0.01;
    document.documentElement.style.setProperty('--vh', `${vh}px`);
});
window.addEventListener('load', (event) => {
    let vh = window.innerHeight * 0.01;
    // Then we set the value in the --vh custom property to the root of the document
    document.documentElement.style.setProperty('--vh', `${vh}px`);
});

window.Tooltip = {
    Init: function () {
        $('[data-toggle="tooltip"]').tooltip({
            trigger: 'hover focus'
        });
    }
}
window.PopOver = {
    Init: function () {
        $('[data-toggle="popover"]').popover();
    }
};
window.Toast = {
    Init: function () {
        $('.toast').toast();
        $('[data-toggle="toast"]').click(function () {
            $(this.dataset.target).toast('show');
        });
    },
    Show: function (ToastId) {
        $('#' + ToastId).toast('show');
    },
    Hide: function (ToastId) {
        $('#' + ToastId).toast('hide');
    }
};
window.Modal = {
    Show: function (id) {
        $('#' + id).modal('show');
    }
};
window.GetDateInPersian = function (date) {
    return moment(date).format('dddd jD jMMMM  jYY');
};
window.SelectPlanDate = {
    SetupInline: function (helper, plans) {
        if (!$(".inline-calendar").hasClass("pwt-datepicker-input-element")) {
            $('.inline-calendar').persianDatepicker({
                inline: true,
                altField: '.DatePicker',
                altFormat: 'YYYY/MM/DD',
                format: 'YYYY/MM/DD',
                initialValueType: 'persian',
                onSelect: function (unix) {
                    helper.invokeMethodAsync("SetTime", unix / 1000);
                },
                navigator: {
                    text: {
                        btnNextText: '',
                        btnPrevText: ''
                    }
                }
            });
            function PlanCountBadge() {
                $(`.table-days *[data-date] span`).removeAttr('data-count');
                for (let p = 0; p < plans.length; p++) {
                    let date = new persianDate(moment(plans[p].date).toDate());
                    let span = $(`.table-days *[data-date="${date.year()},${date.month()},${date.date()}"] span`);
                    span.addClass('text-primary');
                    span.attr('data-toggle', 'tooltip');
                    let PlanCount = span.attr('data-count') ? parseInt(span.attr('data-count')) + 1 : 1;
                    span.attr('data-count', PlanCount);
                    span.attr('data-title', `در این روز ${PlanCount} برنامه تعریف شده.`);
                }
                window.Tooltip.Init();
            }

            PlanCountBadge();
            const observer = new MutationObserver((mutations) => {
                mutations.forEach(m => {
                    if ($(m.target).find('.datepicker-navigator').length > 0)
                        PlanCountBadge()
                })
            });
            observer.observe(document, { attributes: true, childList: true, characterData: true, subtree: true });
        }
    },
    Setup: function (helper, plans) {
        if (!$(".calendar").hasClass("pwt-datepicker-input-element")) {
            $('.calendar').persianDatepicker({
                format: 'YYYY/MM/DD',
                initialValueType: 'persian',
                onSelect: function (unix) {
                    helper.invokeMethodAsync("SetTime", unix / 1000);
                },
                navigator: {
                    text: {
                        btnNextText: "بعدی",
                        btnPrevText: "قبلی"
                    }
                }
            });

            for (let p = 0; p < plans.length; p++) {
                let date = new persianDate(moment(plans[p].date).toDate());
                let span = $(`.table-days *[data-date="${date.year()},${date.month()},${date.date()}"] span`);
                span.addClass('text-primary');
                span.attr('data-toggle', 'tooltip');
                let PlanCount = span.attr('data-count') ? parseInt(span.attr('data-count')) + 1 : 1;
                span.attr('data-count', PlanCount);
                span.attr('data-title', `در این روز ${PlanCount} برنامه تعریف شده‌است.`);
            }
            window.Tooltip.Init();
        }
    }
    //    $(".inline-DatePicker").persianDatepicker({
    //        inline: true,
    //        altField: '.DatePicker',
    //        initialValueType: 'persian',
    //        format: 'YYYY/MM/DD',
    //        onSelect: function (unix) {
    //            helper.invokeMethodAsync("SetTime", unix / 1000);
    //        },
    //        onShow: function () {
    //            for (let p = 0; p < plans.length; p++) {
    //                console.log(new persianDate(moment(plans[p].date).toDate()));
    //                let date = new persianDate(moment(plans[p].date).toDate());
    //                $(`.table-days *[data-date="${date.year()},${date.month()},${date.date()}"] span`).addClass('text-danger');
    //            }
    //        }
    //    });
};

moment.loadPersian();
window.DateFromNow = function () {
    var cells = [];
    cells = document.getElementsByClassName('DateFromNow');
    for (var i = 0; i < cells.length; i++) {
        var item = cells.item(i);
        item.innerHTML = moment(item.dataset.content).fromNow();
    }
};