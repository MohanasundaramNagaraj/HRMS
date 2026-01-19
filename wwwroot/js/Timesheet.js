
const attachDeleteEvent = () => {
  
    document.querySelectorAll('.deleteRow').forEach(button => {
        button.addEventListener('click', function () {
            this.closest('tr').remove();
        });
    });
    
    
};

let apiBaseUrl = "/TimeSheets";
$(document).ready(function () {
    
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
    //
    //$('.timesheet-input').on('change', function (event) {
       
    //});
}

function exportToExcel(employeeName, month, year) {
    let table = document.getElementById("timesheetTable");
    let data = [];
    data.push([`Timesheet for the Month ${month} - ${year}`]);
    data.push([]);
    
    data.push([`Employee Name: ${employeeName}`]);
    data.push([]); 

    let rows = table.querySelectorAll("tbody tr");

    // Get headers except the last one (Actions)
    let headers = Array.from(table.querySelectorAll("thead th")).map(th => th.innerText.trim()).slice(0, -1);
    data.push(headers);

    rows.forEach(row => {
        let rowData = [];
        let cells = row.querySelectorAll("td");

         //Loop through all cells except the last one (Actions)
        for (let i = 0; i < cells.length - 1; i++) {
            let input = cells[i].querySelector("input");
            let select = cells[i].querySelector("select");
            rowData.push(input ? input.value : select ? select.options[select.selectedIndex].text :cells[i].innerText);
        }

        data.push(rowData);
        //for (let i = 0; i < cells.length - 1; i++) {
        //    let input = cells[i].querySelector("input");
        //    let select = cells[i].querySelector("select");

        //    if (input) {
        //        rowData.push(input.value);
        //    } else if (select) {
        //        rowData.push(select.options[select.selectedIndex].text); // or select.value if you want the value
        //    } else {
        //        rowData.push(cells[i].innerText.trim());
        //    }
        //}
    });
    
    let ws = XLSX.utils.aoa_to_sheet(data);
    let wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Timesheet");
    XLSX.writeFile(wb, "Timesheet.xlsx");
}

function exportToExcel(employeeName, month, year) {

    let table = document.getElementById("timesheetTable");
    let data = [];
    data.push([`Timesheet for the Month ${month} - ${year}`]);
    data.push([]);

    data.push([`Employee Name: ${employeeName}`]);
    data.push([]);

    let rows = table.querySelectorAll("tbody tr");

    let headers = Array.from(table.querySelectorAll("thead th")).map(th => th.innerText.trim()).slice(0, -1);
    data.push(headers);

    rows.forEach(row => {
        let rowData = [];
        let cells = row.querySelectorAll("td");

        for (let i = 0; i < cells.length - 1; i++) {
            const cell = cells[i];

            const control = cell.querySelector("input, select, textarea");

            if (control) {
                if (control.tagName === "SELECT") {
                    rowData.push(control.options[control.selectedIndex].text);
                } else {
                    rowData.push(control.value);
                }
            } else {
                rowData.push(cell.innerText.trim());
            }
        }


        data.push(rowData);

    });

    let ws = XLSX.utils.aoa_to_sheet(data);
    let wb = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(wb, ws, "Timesheet" + '_' + month);
    XLSX.writeFile(wb, employeeName + '_' + month + '_' + year + '_' + "Timesheet.xlsx");
}

//function exportToPDF(employeeName, month, year) {
//    const { jsPDF } = window.jspdf;
//    let doc = new jsPDF();

//    doc.text(`Timesheet Report for the month of ${month} - ${year}`, 14, 10);

//    doc.text(`Employee Name: ${employeeName}`, 14, 10);

   

//    let table = document.getElementById("timesheetTable");
//    let data = [];
//    let rows = table.querySelectorAll("tbody tr");

//    let headers = Array.from(table.querySelectorAll("thead th")).map(th => th.innerText.trim()).slice(0, -1);
//    data.push(headers);

//    rows.forEach(row => {
//        let rowData = [];
//        let cells = row.querySelectorAll("td");

//        // Loop through all cells except the last one (Actions)
//        for (let i = 0; i < cells.length - 1; i++) {
//            let input = cells[i].querySelector("input");
//            let select = cells[i].querySelector("select");
//            rowData.push(input ? input.value : select ? select.options[select.selectedIndex].text : cells[i].innerText);

//        }

//        data.push(rowData);
//    });

//    doc.autoTable({
//        head: [headers],
//        body: data.slice(1),
//        startY: 40 // Adjust the start position for the table
//    });

//    doc.save("Timesheet.pdf");
//}

function printTable() {
    let printWindow = window.open("", "", "width=800,height=600");
    printWindow.document.write('<html><head><title>Print</title>');
    printWindow.document.write('<link rel="stylesheet" href="https://cdnjs.cloudflare.com/ajax/libs/twitter-bootstrap/5.3.0/css/bootstrap.min.css">');
    printWindow.document.write('</head><body>');
    printWindow.document.write('<h3 class="text-center">Timesheet Report</h3>');

    let table = document.getElementById("timesheetTable").cloneNode(true);
    let rows = table.querySelectorAll("tr");

    // Remove last column (Actions)
    rows.forEach(row => row.removeChild(row.lastElementChild));

    printWindow.document.write(table.outerHTML);
    printWindow.document.write('</body></html>');
    printWindow.document.close();
    printWindow.print();
}
function update() {
    let rows = $('tr');
    let rowDatas = [];
    $(rows).each(function (i, row) {
        if (row.id != '') {
            getday(row.id);
            let year = '';
            if (isNaN(parseInt($('#yearSelector').val()))) {
                year = new Date().getFullYear() - 2024;
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

            //if (input.tagName === "INPUT") {
            //    const inputs = row.querySelectorAll("input");
            //    if (input.value.toLowerCase().includes("leave") || input.value.toLowerCase().includes("permission")) {
            //        inputs.forEach((input) => {
            //            input.style.color = "red";
            //        });
            //    } else {
            //        inputs.forEach((input) => {
            //            input.style.color = "";
            //        });
            //    }
            //}

            let empid;
            if (isNaN(parseInt($('#empSelector').val()))) {
                empid = 0;
            }
            else {
                empid = parseInt($('#empSelector').val());
            }

            
            if (row && row.querySelector("input[type='date']")?.value) {

                //const description = row.querySelector(".description-input")?.value || "";
                //const jsonDescriptionString = '{"Description": ' + JSON.stringify(description) + '}';
                const data = {
                    Id: 0,
                    YearId: year,
                    UniqueId: row.id,
                    MonthId: month,
                    Date: row.querySelector("input[type='date']")?.value || new Date(),
                    Day: row.querySelector("input[placeholder='Day']")?.value || "",
                    Task: row.querySelector(".task-input")?.value || "",
                    Activity: row.querySelector(".activity-input")?.value || "",
                    Descreption: row.querySelector(".description-input")?.value || "",
                    HoursWorked: parseFloat(row.querySelector(".hours-worked-input")?.value) || 0,
                    EmployeeId: empid
                };
                rowDatas.push(data);
                

            }
        }
    })

    addTimesheet(rowDatas);
};

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
    let activityOptions = `<option value="">Select Activity</option>`;
    activities.forEach(function (activity) {
        activityOptions += `<option value="${activity}">${activity}</option>`;
    }); 
    newRow.innerHTML = `
                                        <td style="width:10%;"><input id='date_${uid}' onchange="getday('${uid}')" onkeydown="return false;"  type="date" value="${row.Date}" class="form-control timesheet-input"></td>
                                        <td style="width:7%;"><input id='day_${uid}' type="text" class="form-control timesheet-input" placeholder="Day" value="${row.Day}" disabled></td>
                                        <td style="width:10%;"><input type="text" class="form-control timesheet-input task-input" placeholder="Task Id" value="${row.Task}"></td>
                                        <td style="width:15%;">
                                                <select class="form-control timesheet-input activity-input">
                                                    ${activityOptions}
                                                </select>
                                            </td>
                                        <td style="width:42%;">
                                           <textarea class="form-control timesheet-input description-input"
                                                placeholder="Task Description">${row.Descreption}</textarea>
                                        </td>
                                        <td style="width:5%;"><input type="number" class="form-control timesheet-input hours-worked-input" placeholder="Hours Worked" value="${row.HoursWorked}"></td>
                                        <td style="width:8%;">
                                            <button class="btn btn-success addRow" onclick="addRow(${undefined},${index + 1});"><i class="material-icons">add</i></button>
                                            <button class="btn btn-danger btn-sm deleteRow" onclick="deleteTimesheet('${uid}')"><i class="material-icons">delete_forever</i></button>
                                        </td>
                                      `;
    attachDeleteEvent();
    inputOnChangeCallback();
    newRow.querySelector('.activity-input').value = row.Activity;
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

    datePicker.min = formatDate(today);
    datePicker.max = formatDate(today);
    
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
            Swal.fire({
                icon: 'success',
                title: 'Updated Successful',
                text: 'Updated Successfully!'
            });
           // alert("Updated Successfully");
        },
        error: function (error) {
            alert('error');
        },
    });
}

function deleteTimesheet(timesheetId) {
    $.ajax({
            url: `${apiBaseUrl}/Delete?uniqueId=${timesheetId}`,
            type: "POST",
            success: function (response) {
                Swal.fire({
                    icon: 'success',
                    title: 'Deleted Successful',
                    text: 'Deleted Successfully!'
                });

            },
            error: function (error) {
                alert('error');
            },
        });
    
}
const table = document.getElementById('timesheetTable');
const filters = table.querySelectorAll('.column-filter');

filters.forEach((input, colIndex) => {
    input.addEventListener('input', () => {
        const filterValues = Array.from(filters).map(f => f.value.toLowerCase().trim());

        Array.from(table.tBodies[0].rows).forEach(row => {
            let showRow = true;

            filterValues.forEach((val, i) => {
                if (!val) return;

                const cell = row.cells[i];
                if (!cell) return;

                let cellText = '';
                const inputElement = cell.querySelector('input');

                if (inputElement && inputElement.type === 'date') {
                   
                    const raw = inputElement.value.trim();

                    
                    const [y, m, d] = raw.split("-");
                    cellText = `${d}-${m}-${y}`; 
                } else if (inputElement) {
                    cellText = inputElement.value.toLowerCase().trim();
                } else {
                    cellText = cell.textContent.toLowerCase().trim();
                }

            
                if (!cellText.includes(val)) {
                    showRow = false;
                }
            });

            row.style.display = showRow ? '' : 'none';
        });
    });
});