$(document).ready(function () {
    $("#myInput").on("keypress", function (e) {
        var key = e.which;
        if (key == 13)
        {
            var value = $(this).val().toLowerCase();
            var url = '/api/albums'
            var searchQuery = value;

            if (searchQuery != null) {
                url = url.concat('?searchquery=', searchQuery);
            }

            $("#albumDiv").empty();

            $.ajax(
                {
                    type: 'GET',
                    accepts: 'application/json',
                    url: url,
                    dataType: 'json',
                    success: function (jsonData) {
                        if (jsonData == null) {
                            alert('no data returned');
                            return;
                        }

                        $.each(jsonData, function (index, album) {

                            var data = {
                                title: album.title,
                                albumid: album.albumId,
                                price: album.price,
                                albumarturl: album.albumArtUrl,
                                artist: album.artist,
                                artistid: album.artistid
                            }

                            var template = $("#albumListTemplate").html();

                            var albumSummaryString = Mustache.render(template, data);

                            $("#albumDiv").append(albumSummaryString);
                        });

                    },
                    error: function (ex) {
                        alert(ex);
                    }
                });

            albums.done(function (data, textStatus, jqXHR) {
                var receiver = $('#links');
                receiver.empty();
                var paginationData = jqXHR.getResponseHeader('X-Pagination');

                paginationDataJSON = JSON.parse(paginationData);

                previousPage = paginationDataJSON['previousPageLink'];
                nextPage = paginationDataJSON['nextPageLink'];
                pageNumber = paginationDataJSON['currentPage'];
                totalPages = paginationDataJSON['totalPages'];
                totalAlbums = paginationDataJSON['totalCount'];

                if (previousPage == null) {
                    $("#prev").prop("disabled", true);
                }
                else {
                    $("#prev").prop("disabled", false);
                }

                if (nextPage == null) {
                    $("#next").prop("disabled", true);
                }
                else {
                    $("#next").prop("disabled", false);
                }

            });

            $("#myInput").val("");
            document.getElementById("pageTitle").innerHTML = "Search results: "+searchQuery;
            return false;
        
        }
    });   
});

