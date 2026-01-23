
let barChart, lineChart, pieChart, stackChart;
function getData() {

    const filterFromDate = document.getElementById("filterFromDate")?.value;
    const filterToDate = document.getElementById("filterToDate")?.value;

    var frm = $("<form hidden action='/TimeSheets/Report' method='get'> </form>");
    $(frm).append("<input type=hidden name=EmployeeID value=" + parseInt($("#empSelector").val()) + " id=EmployeeID>");
    $(frm).append("<input type='hidden' name='FromDate' value=" + filterFromDate + " id='FromDate'>");
    $(frm).append("<input type='hidden' name='ToDate' value=" + filterToDate + " id='ToDate'>");
    $(document).find("body").append(frm);
    setTimeout(function () {
        $(frm).submit();
    }, 100);

}

function exportExcel() {

    const employeeName = $('#employeeName').text() || 'Employee';
    const fromDate = $('#filterFromDate').val();
    const toDate = $('#filterToDate').val();

    const table = document.getElementById('reportTable');
    const wb = XLSX.utils.book_new();

    const ws = XLSX.utils.table_to_sheet(table, { origin: 'A3' });

    const title = `Timesheet report for ${employeeName} - ${formatDate(fromDate)} to ${formatDate(toDate)}`;

    XLSX.utils.sheet_add_aoa(ws, [[title]], { origin: 'A1' });

    ws['!merges'] = [{
        s: { r: 0, c: 0 },
        e: { r: 0, c: table.rows[0].cells.length - 1 }
    }];

    ws['!cols'] = [
        { wch: 15 },
        { wch: 25 },
        { wch: 30 },
        { wch: 15 }
    ];

    XLSX.utils.book_append_sheet(wb, ws, 'Timesheet');

    XLSX.writeFile(wb, 'Timesheet_Report.xlsx');
}

generateFullReport();
function renderReportTable(reportData) {
    const table = document.getElementById("reportTable");
    const thead = table.querySelector("thead tr");
    const tbody = table.querySelector("tbody");

    const groupBy = getSelectedGrouping();

    thead.innerHTML = "";
    if (groupBy.includes("date")) thead.innerHTML += "<th>Date</th>";
    if (groupBy.includes("task")) thead.innerHTML += "<th>Task</th>";
    if (groupBy.includes("activity")) thead.innerHTML += "<th>Activity</th>";
    thead.innerHTML += "<th>Total Hours</th>";

    tbody.innerHTML = "";
    let totalPermissionHours = 0;
    let totalWorkingHours = 0;
    let totalLeaveDays = 0;
    reportData.forEach(r => {
        let row = "<tr>";

        if (groupBy.includes("date")) row += `<td>${r.date}</td>`;
        if (groupBy.includes("task")) row += `<td>${r.task}</td>`;
        if (groupBy.includes("activity")) row += `<td>${r.activity}</td>`;

        row += `<td class="fw-bold text-end">${r.hours.toFixed(2)}</td></tr>`;
        tbody.innerHTML += row;
        if (r.activity == "Leave") {
            totalLeaveDays += 1;
        }
        if (r.activity == "Permission") {
            totalPermissionHours += 1;
        }
        if (r.activity != "Leave" && r.activity != "Permission") {
            totalWorkingHours += r.hours;
        }
       
    });

    $('#totalWorkingHours').text(totalWorkingHours);
    $('#totalPermissionHours').text(totalPermissionHours);
    $('#totalLeaveDays').text(totalLeaveDays);
}

function renderBarChart(dates, seriesMap) {
    barChart = echarts.init(document.getElementById('barChart'));

    barChart.setOption({
        title: { text: 'Hours Worked – Bar Chart' },
        tooltip: { trigger: 'axis' },
        legend: { type: 'scroll' },
        xAxis: { type: 'category', data: dates },
        yAxis: { type: 'value' },
        series: Object.keys(seriesMap).map(name => ({
            name,
            type: 'bar',
            data: seriesMap[name]
        }))
    });
}
function renderLineChart(dates, seriesMap) {
    lineChart = echarts.init(document.getElementById('lineChart'));

    lineChart.setOption({
        title: { text: 'Hours Worked – Line Chart' },
        tooltip: { trigger: 'axis' },
        legend: { type: 'scroll' },
        xAxis: { type: 'category', data: dates },
        yAxis: { type: 'value' },
        series: Object.keys(seriesMap).map(name => ({
            name,
            type: 'line',
            smooth: true,
            data: seriesMap[name]
        }))
    });
}
function renderPieChart(pieMap) {
    pieChart = echarts.init(document.getElementById('pieChart'));

    pieChart.setOption({
        title: { text: 'Hours Distribution – Pie Chart', left: 'center' },
        tooltip: { trigger: 'item' },
        legend: { bottom: 0, type: 'scroll' },
        series: [{
            type: 'pie',
            radius: '60%',
            data: Object.keys(pieMap).map(key => ({
                name: key,
                value: pieMap[key]
            }))
        }]
    });

    //pieChart.setOption({
    //    title: { text: 'Hours Distribution – Pie Chart',left: 'center' },
    //    tooltip: { trigger: 'item'},
    //    legend: {
    //        orient: 'vertical',
    //        right: '1%',
    //        top: 'middle',
    //        type: 'scroll'
    //    },
    //    series: [{
    //        type: 'pie',
    //        radius: '60%',
    //        center: ['40%', '50%'], 
    //        data: Object.keys(pieMap).map(key => ({
    //            name: key,
    //            value: pieMap[key]
    //        }))
    //    }]
    //});
}
function renderStackChart(dates, seriesMap) {
    stackChart = echarts.init(document.getElementById('stackChart'));

    stackChart.setOption({
        title: { text: 'Hours Worked – Stacked Chart' },
        tooltip: { trigger: 'axis' },
        legend: { type: 'scroll' },
        xAxis: { type: 'category', data: dates },
        yAxis: { type: 'value' },
        series: Object.keys(seriesMap).map(name => ({
            name,
            type: 'bar',
            stack: 'total',
            data: seriesMap[name]
        }))
    });
}

function bindFilterEvents() {

    document.getElementById("filterTask")
        .addEventListener("input", onFilterChange);

    document.getElementById("filterActivity")
        .addEventListener("input", onFilterChange);

    document.querySelectorAll(".group-by")
        .forEach(cb => cb.addEventListener("change", onFilterChange));
}

function onFilterChange() {
    generateFullReport();
}

const reportModal = $('#reportModal');
function generateFullReport() {
    bindFilterEvents();
    const reportData = generateTimesheetReport();

    renderReportTable(reportData);

    const { dates, seriesMap, pieMap } = prepareChartData(reportData);

    // renderBarChart(dates, seriesMap);
    // renderLineChart(dates, seriesMap);
    renderPieChart(pieMap);
    // renderStackChart(dates, seriesMap);
}

function getSelectedGrouping() {
    return Array.from(document.querySelectorAll(".group-by:checked"))
        .map(cb => cb.value);
}

$('#reportModal').on('shown.bs.modal', function () {
    setTimeout(() => {
        $('#employeeNameText').text(employeeName.textContent);

        barChart?.resize();
        lineChart?.resize();
        pieChart?.resize();
        stackChart?.resize();
    }, 300);
});

window.addEventListener('resize', () => {
    barChart?.resize();
    lineChart?.resize();
    pieChart?.resize();
    stackChart?.resize();
});

function loadActivityFilter() {
    const activitySet = new Set();

    document
        .querySelectorAll("#timesheetTable .activity-input")
        .forEach(sel => {
            if (sel.value) activitySet.add(sel.value);
        });

    const ddl = document.getElementById("filterActivity");
    ddl.innerHTML = `<option value="">All Activities</option>`;

    activitySet.forEach(a => {
        ddl.innerHTML += `<option value="${a}">${a}</option>`;
    });
}
function generateTimesheetReport() {

    const rows = timeSheetData;

    let firstDate = rows[0]?.Date;

    const _filterFromDate = document.getElementById("filterFromDate");
    if (firstDate) {
        const dateObj = new Date(firstDate);

        const monthYear = dateObj.toLocaleString('en-US', {
            month: 'long',
            year: 'numeric'
        });

        $('#tsRptMonYear').text(monthYear);

        if (_filterFromDate) {
           // _filterFromDate.min = firstDate;

            const lastDateOfMonth = new Date(
                dateObj.getFullYear(),
                dateObj.getMonth() + 1,
                0
            );

            const maxDate = lastDateOfMonth.toISOString().split('T')[0];

        }
    }

    const filterTask = document.getElementById("filterTask")?.value.toLowerCase();
    const filterActivity = document.getElementById("filterActivity")?.value;
    const groupBy = getSelectedGrouping();

    const reportMap = {};

    for (let row of rows) {

        let formattedDate = formatDate(row.Date)

        const date = formattedDate;
        const task = row.Task;
        const activity = row.Activity;
        const hours = row.HoursWorked;

        if (filterTask && !task.toLowerCase().includes(filterTask)) continue;
        if (filterActivity && activity !== filterActivity) continue;

        const keyParts = [];
        if (groupBy.includes("date")) keyParts.push(date);
        if (groupBy.includes("task")) keyParts.push(task);
        if (groupBy.includes("activity")) keyParts.push(activity);

        const key = keyParts.join("|");

        if (!reportMap[key]) {
            reportMap[key] = {
                date: groupBy.includes("date") ? date : "All",
                task: groupBy.includes("task") ? task : "All",
                activity: groupBy.includes("activity") ? activity : "All",
                hours: 0
            };
        }

        reportMap[key].hours += hours;
    }

    return Object.values(reportMap);
}

function formatDate(date) {
    const [year, month, day] = date.split('T')[0].split('-');

    return `${day}-${month}-${year}`;
}

function prepareChartData(reportData) {
    const dates = [...new Set(reportData.map(r => r.date))].sort();

    const seriesMap = {};
    const pieMap = {};

    reportData.forEach(r => {
        const key = `${r.task} - ${r.activity}`;

        if (!seriesMap[key]) {
            seriesMap[key] = new Array(dates.length).fill(0);
        }

        const dateIndex = dates.indexOf(r.date);
        seriesMap[key][dateIndex] += r.hours;

        pieMap[key] = (pieMap[key] || 0) + r.hours;
    });

    return { dates, seriesMap, pieMap };
}


