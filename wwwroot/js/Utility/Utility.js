$(document).ready(function () {
    $('.page-loader-wrapper').hide();
});

const AttendanceStatus = {
    Present: 0,
     HalfDay: 1,
    Absent: 2,
    Weekend: 3,
    Holiday: 4,
    PermissionNeeded: 5,
    PendingCheckOut: 6
};