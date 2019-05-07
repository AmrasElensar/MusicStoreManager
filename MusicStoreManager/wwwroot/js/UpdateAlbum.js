$(document).ready(function () {

    $("#updateAlbum").click(function () {

        var albumOfTheWeekBool;
        var albumId = $('input[name=albumId]').val();
        var artistId = $('input[name=artistId]').val();

        if ($("input[name='albumOfTheWeek']").attr('checked')) {

            albumOfTheWeekBool = true;
        }
        else {
            albumOfTheWeekBool = false;
        }

        var formData = {
            GenreId: $('#genreSelector').children("option:selected").val(),
            Title: $('input[name=title]').val(),
            Price: $('input[name=price]').val(),
            AlbumArtUrl: $('input[name=albumArtUrl]').val(),
            IsAlbumOfTheWeek: albumOfTheWeekBool
        };



        $.ajax({
            type: 'PUT', 
            url: '/api/albums/artist/' + artistId + '/' + albumId,
            data: JSON.stringify(formData), 
            contentType: "application/json; charset=utf-8",
            dataType: 'json', 
            encode: true
        })

            .done(function (data) {

                alert("Album" + albumId + "updated succesfully.");
                window.location.reload();
            });

        
    });

});