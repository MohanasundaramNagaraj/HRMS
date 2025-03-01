
const attachDeleteEvent = () => {
    document.querySelectorAll('.deleteRow').forEach(button => {
        button.addEventListener('click', function () {
            this.closest('tr').remove();
        });
    });
};

let apiBaseUrl = "/TimeSheets";
$(document).ready(function () {
    debugger;
    console.log(timeSheetData);

    if (timeSheetData.length > 0) {
        $(timeSheetData).each(function (i, v) {
            addRow(v, i);
        });
    }
    else {
        let timesheetData = {
            UniqueId: "",
            Date: "",
            Day: "",
            Task: "",
            Activity: "",
            Descreption: "",
            HoursWorked: 0,

        };
        addRow(timesheetData, 0);
    }

    inputOnChangeCallback();
});

function inputOnChangeCallback() {
    $('.timesheet-input').on('change', function (event) {

        const input = event.target;
        const row = input.closest("tr");
        getday(row.id);
        let year = '';
        if (isNaN(parseInt($('#yearSelector').val()))) {
            year = new Date().getFullYear()
        }
        else {
            year = parseInt($('#yearSelector').val());
        }

        let month = '';
        if (isNaN(parseInt($('#monthSelector').val()))) {
            month = new Date().getMonth() + 1;
        }
        else {
            month = parseInt($('#monthSelector').val());
        }

        if (input.tagName === "INPUT") {
            const inputs = row.querySelectorAll("input");
            if (input.value.toLowerCase().includes("leave") || input.value.toLowerCase().includes("permission")) {
                inputs.forEach((input) => {
                    input.style.color = "red";
                });
            } else {
                inputs.forEach((input) => {
                    input.style.color = "";
                });
            }
        }

        let empid;
        if (isNaN(parseInt($('#empSelector').val()))) {
            empid = 0;
        }
        else {
            empid = parseInt($('#empSelector').val());
        }


        if (row) {
            const data = {
                Id: 0,
                YearId: year,
                UniqueId: row.id,
                MonthId: month,
                Date: row.querySelector("input[type='date']")?.value || "",
                Day: row.querySelector("input[placeholder='Day']")?.value || "",
                Task: row.querySelector(".task-input")?.value || "",
                Activity: row.querySelector(".activity-input")?.value || "",
                Descreption: row.querySelector(".description-input")?.value || "",
                HoursWorked: parseInt(row.querySelector(".hours-worked-input")?.value) || 0,
                EmployeeId: empid
            };

            addTimesheet(data);

        }
    });
}
function addRow(row, index) {
    let uid;
    if (row == undefined || row.UniqueId == "") {
        row = {
            UniqueId: "",
            Date: "",
            Day: "",
            Task: "",
            Activity: "",
            Descreption: "",
            HoursWorked: 0,

        };
        uid = generateUUIDv4();
    } else {
        uid = row.UniqueId;

        row.Date = row.Date.replaceAll('T00:00:00', '');
    }

    const table = document.getElementById('timesheetTable').getElementsByTagName('tbody')[0]; 
    const newRow = table.insertRow(index);
    newRow.id = uid;
    newRow.innerHTML = `
                                        <td style="width:10%;"><input id='date_${uid}' onchange="getday('${uid}')" type="date" value="${row.Date}" class="form-control timesheet-input"></td>
                                        <td style="width:7%;"><input id='day_${uid}' type="text" class="form-control timesheet-input" placeholder="Day" value="${row.Day}" disabled></td>
                                        <td style="width:10%;"><input type="text" class="form-control timesheet-input task-input" placeholder="Task Id" value="${row.Task}"></td>
                                        <td style="width:15%;"><input type="text" class="form-control timesheet-input activity-input" placeholder="Activity" value="${row.Activity}"></td>
                                        <td style="width:42%;"><input type="text" class="form-control timesheet-input description-input" placeholder="Task Description" value="${row.Descreption}"/></td>
                                        <td style="width:5%;"><input type="number" class="form-control timesheet-input hours-worked-input" placeholder="Hours Worked" value="${row.HoursWorked}"></td>
                                        <td style="width:8%;">
                                            <button class="btn btn-success addRow" onclick="addRow(${undefined},${index + 1});"><i class="material-icons">add</i></button>
                                            <button class="btn btn-danger btn-sm deleteRow" onclick="deleteTimesheet('${uid}')"><i class="material-icons">delete_forever</i></button>
                                        </td>
                                      `;
    attachDeleteEvent();
    inputOnChangeCallback();

    const inputs = newRow.querySelectorAll("input");
    if (row.Descreption.toLowerCase().includes("leave") || row.Descreption.toLowerCase().includes("permission")) {
        inputs.forEach((input) => {
            input.style.color = "red";
        });
    } else {
        inputs.forEach((input) => {
            input.style.color = "";
        });
    }


    const today = new Date();

    const firstDay = new Date(today.getFullYear(), today.getMonth(), 1);
    const lastDay = new Date(today.getFullYear(), today.getMonth() + 1, 0);

    const datePicker = document.getElementById('date_' + uid);

    const formatDate = (date) => {
        const year = date.getFullYear();
        const month = String(date.getMonth() + 1).padStart(2, '0');
        const day = String(date.getDate()).padStart(2, '0');
        return `${year}-${month}-${day}`;
    };

    datePicker.min = formatDate(firstDay);
    datePicker.max = formatDate(lastDay);
};


function getday(uid) {
    const dateInput = document.getElementById("date_" + uid);
    const dayOutput = document.getElementById("day_" + uid);

    const selectedDate = new Date(dateInput.value);
    if (!isNaN(selectedDate)) {
        const days = ["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"];

        const dayName = days[selectedDate.getDay()];
        dayOutput.value = dayName;
    } else {
        dayOutput.value = "";
    }


}


attachDeleteEvent();

function getData(obj) {

    var frm = $("<form hidden action='/TimeSheets/Entry' method='get'> </form>");
    $(frm).append("<input type=hidden name=EmployeeID value=" + parseInt($("#empSelector").val()) + " id=EmployeeID>");
    $(frm).append("<input type='hidden' name='Month' value=" + parseInt($("#monthSelector").val()) + " id='Month'>");
    $(frm).append("<input type='hidden' name='Year' value=" + parseInt($("#yearSelector").val()) + " id='Year'>");
    $(document).find("body").append(frm);
    setTimeout(function () {
        $(frm).submit();
    }, 100);

}


function addTimesheet(timesheetData) {
    let timeSheet = JSON.stringify(timesheetData);

    $.ajax({
        url: `${apiBaseUrl}/Update`,
        type: "POST",
        data: { timesheet: timeSheet },
        success: function (response) {

        },
        error: function (error) {

        },
    });
}

function deleteTimesheet(timesheetId) {
    $.ajax({
        url: `${apiBaseUrl}/Delete?uniqueId=${timesheetId}`,
        type: "POST",
        success: function (response) {

        },
        error: function (error) {

        },
    });
}