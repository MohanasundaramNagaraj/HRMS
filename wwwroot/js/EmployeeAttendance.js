//$("select").formSelect();
function getData(obj) {

    var frm = $("<form hidden action='/EmployeeAttendance/Index' method='get'> </form>");
    $(frm).append("<input type=hidden name=EmployeeID value=" + parseInt($("#empSelector").val()) + " id=EmployeeID>");
    $(frm).append("<input type='hidden' name='Month' value=" + parseInt($("#monthSelector").val()) + " id='Month'>");
    $(frm).append("<input type='hidden' name='Year' value=" + parseInt($("#yearSelector").val()) + " id='Year'>");
    $(document).find("body").append(frm);
    setTimeout(function () {
        $(frm).submit();
    }, 100);

}

let calendar;
//var Draggable = FullCalendarInteraction.Draggable;
let date_picker;

var containerEl = document.getElementById("external-events");
var addEvent = document.getElementById("add-event");
var editEvent = document.getElementById("edit-event");
var addEventTitle = document.getElementById("addEventTitle");
var editEventTitle = document.getElementById("editEventTitle");

$(document).ready(function () {
    month = parseInt(month);
    year = parseInt(year);
    initCalendar();
    addEvetClick();
    editEvetClick();
    //flatpickr("#starts-at", {
    //    enableTime: true,
    //    allowInput: true,
    //    dateFormat: "Y-m-d H:i",
    //    onOpen: function (selectedDates, dateStr, instance) {
    //        instance.setDate(instance.input.value, false);
    //    },
    //});
    //flatpickr("#ends-at", {
    //    enableTime: true,
    //    allowInput: true,
    //    dateFormat: "Y-m-d H:i",
    //    onOpen: function (selectedDates, dateStr, instance) {
    //        instance.setDate(instance.input.value, false);
    //    },
    //});
});

function initCalendar() {
    debugger;
    var calendarEl = $("#calendar").get(0);
    calendar = new FullCalendar.Calendar(calendarEl, {
        plugins: ["interaction", "dayGrid", "timeGrid"],
        header: {
            left: "",
            //left: "prev,next today",
            center: "title",
            //right: "dayGridMonth,timeGridWeek,timeGridDay",
            right: ""
        },
        editable: false,
        droppable: false,
        navLinks: true,
        eventLimit: true,
        weekNumberCalculation: "ISO",
        displayEventEnd: true,
        lazyFetching: true,
        selectable: true,

        eventMouseEnter: function (info) {
            $(info.el).attr("id", info.event.id);

            $("#" + info.event.id).popover({
                template:
                    '<div class="popover" role="tooltip"><div class="arrow"></div><h4 class="popover-header"></h4><div class="popover-body"></div></div>',
                title: info.event.title,
                content: info.event.extendedProps.description,
                placement: "top",
                html: true,
            });
            $("#" + info.event.id).popover("show");
            $(".popover .popover-header").css(
                "color",
                $(info.el).css("background-color")
            );
        },
        eventMouseLeave: function (info) {
            $("#" + info.event.id).popover("hide");
        },
        views: {
            dayGridMonth: {
                eventLimit: 3,
            },
        },

        events: events(),

        select: function (start, end) {

            addEvent.style.display = "block";
            editEvent.style.display = "none";
            addEventTitle.style.display = "block";
            editEventTitle.style.display = "none";

            let modalHtml = fn_showModal();
            $('body').append(modalHtml);

            clearModalForm();
            $(".modal").modal("show");
        },
        eventClick: function (info) {
            addEvent.style.display = "none";
            editEvent.style.display = "block";
            addEventTitle.style.display = "none";
            editEventTitle.style.display = "block";

            let startDate = moment(info.event.start).format("YYYY-MM-DD HH:mm:ss");
            let endDate = moment(info.event.end).format("YYYY-MM-DD HH:mm:ss");

            let modalHtml = fn_showModal();
            $('body').append(modalHtml);

            // console.log(info.event.extendedProps.description);
            $(".modal").modal("show");
            $(".modal").find("#id").val(info.event.id);
            $(".modal").find("#title").val(info.event.title);
            $(".modal").find("#starts-at").val(startDate);
            $(".modal").find("#ends-at").val(endDate);
            $("#categorySelect").val(info.event.classNames[0]);
            $(".modal")
                .find("#eventDetails")
                .val(info.event.extendedProps.description);
        },
    });

    calendar.render();

    var newDate = new Date(year, month - 1, 1); // Change this to the desired date (YYYY-MM-DD)
    calendar.gotoDate(newDate);
    $('.fc-time').remove();
    $('#calendar').css({
        'margin-top': '-50px'
    })
    // $('.fc-header-toolbar').remove();
}

function fn_changeEntryType() {

}

function fn_showModal() {
    let uid = generateUUIDv4();
    $('.requestModal').remove();
    return `<div class="modal fade requestModal" id="requestModal" tabindex="-1" role="dialog" aria-hidden="true">
    <div class="modal-dialog modal-dialog-centered" role="document">
        <div class="modal-content">
            <div class="modal-header">
                <h5 class="modal-title" id="addEventTitle">Add Request</h5>
                <h5 class="modal-title" id="editEventTitle">View Request</h5>
                <button type="button" class="close" data-bs-dismiss="modal"
                        aria-label="Close">
                    <span aria-hidden="true">&times;</span>
                </button>
            </div>
            <div class="modal-body">
                <form class="">
                    <input type="hidden" id="id" name="id">
                   
                    <div class="row">
                        <div class="col-md-12 mb-4">
                            <div class="form-group default-select">
                                <label>Request Type</label>
                                <select class="form-control" id="entrytype" style="padding-left: 10px;" onchange="fn_changeEntryType(this.value)">
                                    <option value="" disabled selected>Choose your option</option>
                                    <option value="@Constants.EntryTypes.Permission">@Constants.EntryTypes.Permission</option>
                                    <option value="@Constants.EntryTypes.OnDuty">@Constants.EntryTypes.OnDuty</option>
                                    <option value="@Constants.EntryTypes.Leave">@Constants.EntryTypes.Leave</option>
                                </select>
                            </div>
                        </div>
                    </div>
                    <div class="row">
                        <div class="col-sm-6" id="fromDateDiv">
                            <div class="form-group form-float">
                                <div class="form-line">
                                    <label>From Date</label>
                                    <input id="from_datetime" type="datetime-local" class="validate form-control" placeholder="From Date">
                                    <input id="from_date" type="date" class="validate form-control" placeholder="From Date">

                                </div>
                            </div>
                        </div>

                        <div class="col-sm-6" id="toDateDiv">
                            <div class="form-group form-float">
                                <div class="form-line">
                                    <label>To Date</label>
                                    <input id="to_datetime" type="datetime-local" class="validate form-control" placeholder="To Date">
                                    <input id="to_date" type="date" class="validate form-control" placeholder="To Date">

                                </div>
                            </div>
                        </div>
                    </div>

                    <div>
                        <div class="form-group">
                            <label>Descreption</label>
                            <textarea id="eventDetails" name="eventDetails" placeholder="Enter Details"
                                      class="form-control"></textarea>
                        </div>
                    </div>
                    <div class="modal-footer bg-whitesmoke pr-0">
                        <button type="button" class="btn btn-primary" id="add-event">
                            Add
                            Request
                        </button>
                        <button type="button" class="btn btn-round btn-primary" id="edit-event">
                            Edit
                            Request
                        </button>
                        <button type="button" id="close" class="btn btn-danger"
                                data-bs-dismiss="modal">
                            Close
                        </button>
                    </div>
                </form>
            </div>
        </div>
    </div>
</div>`;
}

function clearModalForm() {
    var input = document.querySelectorAll('input[type="text"]');
    var textarea = document.getElementsByTagName("textarea");
    for (i = 0; i < input.length; i++) {
        input[i].value = "";
    }
    for (j = 0; j < textarea.length; j++) {
        textarea[j].value = "";
        i;
    }
}

function addEvetClick() {
    $("#add-event").on("click", function (event) {
        var title = $("#title").val();
        var eventDetails = document.getElementById("eventDetails").value;
        var category = $("#categorySelect").find(":selected").val();
        var randomID = randomIDGenerate(
            10,
            "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ"
        );
        calendar.addEvent({
            id: randomID,
            title: title,
            start: $("#starts-at").val(),
            end: $("#ends-at").val(),
            className: category,
            description: eventDetails,
        });
        // Clear modal inputs
        $(".modal").find("input").val("");
        // hide modal
        $(".modal").modal("hide");
    });
}

function editEvetClick() {
    $("#edit-event")
        .off("click")
        .on("click", function (event) {
            event.preventDefault();
            var category = $("#categorySelect").find(":selected").val();

            var event2 = calendar.getEventById(document.getElementById("id").value);

            var eventDetails = document.getElementById("eventDetails").value;
            var category = $("#categorySelect").find(":selected").val();

            event2.setExtendedProp("id", document.getElementById("id").value + "");
            event2.setProp("title", document.getElementById("title").value + "");
            event2.setStart(
                moment(document.getElementById("starts-at").value).format(
                    "YYYY-MM-DD HH:mm:ss"
                )
            );
            event2.setEnd(
                moment(document.getElementById("ends-at").value).format(
                    "YYYY-MM-DD HH:mm:ss"
                )
            );
            event2.setProp("classNames", category);
            event2.setExtendedProp("description", eventDetails);
            $(".modal").modal("hide");
        });
}
function randomIDGenerate(length, chars) {
    var result = "";
    for (var i = length; i > 0; --i)
        result += chars[Math.round(Math.random() * (chars.length - 1))];
    return result;
}

function events() {
    var employeeAttendanceRecordsArray = JSON.parse(employeeAttendanceRecords);

    var formattedEvents = [];

    $.each(employeeAttendanceRecordsArray, function (index, record) {
        var statusClass = "";
        let title = '';
        title = record.CheckInTimeInString + ' - ' + record.CheckOutTimeInString;
        let dayName = new Date(record.Date).toLocaleDateString('en-US', { weekday: 'long' });
        let description = formatDate(record.Date) + "<br>" + dayName;
        description += record.WorkingHours ? "<br>Working Hours: " + record.WorkingHours + "." : "";



        if (record.Id != 0 || record.Status == AttendanceStatus.Present) {
            statusClass = "fc-event-success"; // Present
            if (roles.includes("Admin") || roles.includes("SuperAdmin") || roles.includes("User")) {
                description += "<br>CheckIn IP: " + (record.IP || "Not available") +
                    ",<br>CheckOut IP: " + (record.CheckOutMadeSystemIP || "Not available") + ".";
            }
        } else if (record.Status == AttendanceStatus.Absent) {
            statusClass = "fc-event-danger"; // Absent
            title = 'Absent';
        } else if (record.Status == AttendanceStatus.Holiday) {
            statusClass = "fc-event-warning"; // Holiday
            title = 'Holiday';
        } else if (record.Status == AttendanceStatus.PermissionNeeded) {
            statusClass = "fc-event-success"; // Taken Permission
        } else if (record.Status == AttendanceStatus.Weekend) {
            statusClass = "fc-event-warning"; // Holiday
            title = 'Holiday';
        }
        else if (record.Status == AttendanceStatus.LeaveRequested){
            statusClass = "fc-event-warning";
            title = 'Leave Requested';
        }
        else if (record.Status == AttendanceStatus.LeaveRequestApproved) 
        {
            statusClass = "fc-event-warning";
            title = 'Leave Request Approved';
        }

        if (record.Status == AttendanceStatus.Weekend
            || record.Status == AttendanceStatus.Holiday
            || record.Status == AttendanceStatus.LeaveRequested
            || record.Status == AttendanceStatus.LeaveRequestApproved
            || new Date(record.Date) < new Date()
        ) {
            var eventObj = {
                id: "event" + (index + 1),
                // title: formatDate(record.Date),
                title: title,
                start: record.CheckInDateTime ? new Date(record.CheckInDateTime) : new Date(record.Date),
                end: record.CheckOutDateTime ? new Date(record.CheckOutDateTime) : new Date(new Date(record.Date).setHours(23, 59)),
                className: statusClass, // Set the class name based on attendance status
                description: description
            };

            // Push the event object to the formattedEvents array
            formattedEvents.push(eventObj);
        }

    });

    console.log(formattedEvents);
    return formattedEvents;
}

function formatDate(dateString) {
    const options = { day: '2-digit', month: 'short', year: 'numeric' };
    return new Date(dateString).toLocaleDateString('en-US', options);
}