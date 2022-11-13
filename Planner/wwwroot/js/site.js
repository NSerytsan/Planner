$(function () {
    $(document).ready(function () {
        $("#search-box").autocomplete({
            source: function (request, response) {
                $.ajax({
                    url: '/api/PlanApi/search',
                    data: { "name": request.term },
                    dataType: "json",
                    success: function (data) {
                        response($.map(data, function (item) {
                            return item;
                        }))
                    }
                });
            }
        });
    });
});