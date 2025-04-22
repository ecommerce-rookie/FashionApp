$.debounce = function (fn, delay) {
    let timer = null;
    return function (...args) {
        clearTimeout(timer);
        timer = setTimeout(() => fn.apply(this, args), delay);
    };
};

$(function () {
    const $form = $('#filter-form');
    const $output = $('#product-list');

    const debouncedFilter = $.debounce(doFilter, 300);

    $form.on('input', 'input[type="text"]', debouncedFilter);

    $form.on('change', 'input, select', doFilter);

    $(document).on('click', '.page-nav__link, .page-nav__button', function (e) {
        e.preventDefault();

        const page = $(this).data('page');
        if (!page || $(this).attr('disabled')) return;

        $('#page-input').val(page);

        doFilter();
    });

    function doFilter() {
        const dataArray = $form.serializeArray();
        const filterData = {};
        $.each(dataArray, function (_, field) {
            if (filterData[field.name]) {
                if (!Array.isArray(filterData[field.name])) {
                    filterData[field.name] = [filterData[field.name]];
                }
                filterData[field.name].push(field.value);
            } else {
                filterData[field.name] = field.value;
            }
        });

        $.ajax({
            url: '/products?handler=ProductsPartial',
            data: filterData,
            type: 'GET',
            success: function (html) {
                $output.html(html);
            },
            error: function (err) {
                console.error('Filter error', err);
            }
        });
    }
});
