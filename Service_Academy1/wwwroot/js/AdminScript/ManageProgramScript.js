function openDenyModal(programId) {
    $('#programId').val(programId);  // Set enrollment ID in the hidden input
    $('#denyModal').modal('show');         // Show the modal
}
$(document).ready(function () {
    $('#viewDescriptionModal').on('show.bs.modal', function (event) {
        var button = $(event.relatedTarget); // Button that triggered the modal
        var programId = button.data('program-id'); // Extract program ID
        var title = button.data('title'); // Extract program title
        var description = button.data('description'); // Extract program description
        var agenda = button.data('agenda'); // Extract program agenda
        var sdg = button.data('sdg'); // Extract program agenda
        var modal = $(this);

        // Set the modal title
        modal.find('.modal-title').text('Description for ' + title);

        // Populate the agenda and description fields
        modal.find('#programAgenda').text(agenda);
        modal.find('#programSDG').text(sdg);
        modal.find('#programDescription').text(description);
    });
});