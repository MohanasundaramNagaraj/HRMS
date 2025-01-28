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

function generateUUIDv4() {
    return ([1e7] + -1e3 + -4e3 + -8e3 + -1e11).replace(/[018]/g, c =>
        (c ^ crypto.getRandomValues(new Uint8Array(1))[0] & 15 >> c / 4).toString(16)
    );
}