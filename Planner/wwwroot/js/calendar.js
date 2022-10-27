$(function () {
    var apiUrl = '/api/PlanApi';
    var calendar = new FullCalendar.Calendar(document.getElementById('calendar'), {
        headerToolbar: {
            left: 'prev,next today',
            right: 'dayGridMonth,dayGridWeek,list',
            center: 'title',
        },
        selectable: true,
        themeSystem: 'bootstrap5',
        editable: true,
        eventSources: [{
            events: function (info, successCallback, failureCallback) {
                $.ajax({
                    url: apiUrl,
                    method: 'GET',
                    dataType: 'json',
                    success: function (response) {
                        successCallback(response);
                    },
                    error: function (response) {
                        failureCallback(response);
                    }
                });
            }
        }],
        eventClick: function (info) {
            var eventDetails = $('#event-details-modal');
            var id = info.event.id;
            eventDetails.find('#title').text(info.event.title);
            eventDetails.find('#description').text(info.event.extendedProps.description);
            eventDetails.find('#start').text(moment(info.event.start).format('YYYY-MM-DD HH:mm'));
            eventDetails.find('#end').text(moment(info.event.end).format('YYYY-MM-DD HH:mm'));
            eventDetails.find('#edit,#delete').attr('data-id', id);
            eventDetails.modal('show');

        },
        select: function (info) {
            var newPlanDialog = $('#plan-form-modal');
            newPlanDialog.find('[name="start_datetime"]').val(moment(info.start).format('YYYY-MM-DDTHH:mm'));
            newPlanDialog.find('[name="end_datetime"]').val(moment(info.end).format('YYYY-MM-DDTHH:mm'));
            newPlanDialog.modal('show');
        }
    });
    calendar.render()

    $('#plan-form-modal').on('hidden.bs.modal', function (e) {
        $(this).find('#plan-form')[0].reset();
        $(this).find('#save').attr('data-id', '');
    });

    $('#edit').click(function () {
        var id = $(this).attr('data-id');
        var event = calendar.getEventById(id);
        var editPlanDialog = $('#plan-form-modal');
        $('#event-details-modal').modal('hide');
        editPlanDialog.find('#save').attr('data-id', id);
        editPlanDialog.find('#title').val(event.title);
        editPlanDialog.find('#description').val(event.extendedProps.description);
        editPlanDialog.find('[name="start_datetime"]').val(moment(event.start).format('YYYY-MM-DDTHH:mm'));
        editPlanDialog.find('[name="end_datetime"]').val(moment(event.end).format('YYYY-MM-DDTHH:mm'));
        editPlanDialog.modal('show');
    });

    $('#delete').click(function () {
        var id = $(this).attr('data-id');
        var _conf = confirm("Are you sure to delete this plan?");
        if (_conf === true) {
            var url = apiUrl + '/' + id;
            $.ajax({
                type: 'DELETE',
                url: url,
                success: function (response) {
                    $('#event-details-modal').modal('hide');
                    var event = calendar.getEventById(id);
                    event.remove();
                },
                error: function () {
                    alert('Delete Plan Failed');
                }
            });
        }
    });

    $('#save').click(function () {
        var id = $(this).attr('data-id');
        var plan = {};
        var planDialog = $('#plan-form-modal');
        var method = 'PUT';
        var url = apiUrl;

        if (id === null || id === undefined || !id) {
            method = 'POST';
        }
        else {
            plan.id = id;
            url = apiUrl + '/' + id;
        }

        plan.title = planDialog.find('#title').val();
        plan.description = planDialog.find('#description').val();
        plan.start = planDialog.find('[name="start_datetime"]').val();
        plan.end = planDialog.find('[name="end_datetime"]').val();
        plan.color = "";

        // FIXME - Validation
        
        $.ajax({
            url: url,
            method: method,
            contentType: "application/json",
            dataType: 'json',
            processData: false,
            data: JSON.stringify(plan),
            success: function (response) {
                calendar.refetchEvents();
                planDialog.modal('hide');
            },
            error: function () {
                alert('Save Plan Failed');
            }
        });
    });
})