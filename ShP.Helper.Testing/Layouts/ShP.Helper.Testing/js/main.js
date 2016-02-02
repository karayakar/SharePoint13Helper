var data;
function ParseListItemData() {
    var sData = $('#ListItemData span').text();
    data = jQuery.parseJSON(sData);
    console.log(data);

    var t = data[0].PublishingHTML;
    $('#ctl00_SPWebPartManager1_g_7a09476e_e2a8_46ae_9447_bcb994530504_InfoLabel').html(t);

    var d = new Date(parseInt(data[0].Created.slice(6, -2)));
    console.log(d);
}