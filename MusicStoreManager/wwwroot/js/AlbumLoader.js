var pageNumber;
var pageSize;
var totalPages;
var previousPage;
var nextPage;
var totalAlbums;
var _genre;
var searchQuery;

$(document).ready(function () {
    if ($("#albumDiv").length > 0) {
        LoadMoreAlbums();
    }
});

var prevButton = document.getElementById("prev");
var nextButton = document.getElementById("next");

prevButton.onclick = function () {
    LoadMoreAlbums(_genre,null,previousPage);
}

nextButton.onclick = function () {
    LoadMoreAlbums(_genre,nextPage,null);
}


$("#myInput").on("keypress", function (e) {
    var key = e.which;
    if (key == 13) {
        var value = $(this).val().toLowerCase();
        searchQuery = value;
        nextPage = null;
        previousPage = null
        _genre = null;

        if (searchQuery != null) {
            LoadMoreAlbums(_genre, nextPage, previousPage, searchQuery)
        }

        
    }
});

function LoadMoreAlbums(genre, nextpg, prevpg, search) {

    var url = '/api/albums'
    document.getElementById("pageTitle1").innerHTML = "Check 'm all";
    document.getElementById("pageTitle2").innerHTML = "All Genres";

    if (search != null) {
        url = url + '?searchquery=' + search;
        document.getElementById("pageTitle1").innerHTML = "Search results:"
        document.getElementById("pageTitle2").innerHTML = search;
        $("#myInput").val("");

    }

    if (genre != null) {
        url = url + "?genre=" + genre;
        document.getElementById("pageTitle1").innerHTML = "Genre:"
        document.getElementById("pageTitle2").innerHTML = genre;
        _genre = genre
    }

    if (genre == 'Sexy Music') {
        window.location = '/Album/SexyMusic';
    }

    if (prevpg != null) {
        url = prevpg;
    }

    if (nextpg != null) {
        url = nextpg;
    }

    var albums = $.ajax(
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

                $("#albumDiv").empty();

                $.each(jsonData, function (index, album) {

                    var data = {
                        title: album.title,
                        albumid: album.albumId,
                        price: album.price,
                        albumarturl: album.albumArtUrl,
                        artist: album.artist,
                        artistid: album.artistId
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
            $("#prevBtn").attr("disabled", true);
            urlPrev = null;
        }
        else {
            $("#prevBtn").removeAttr("disabled");
        }

        if (nextPage == null) {
            $("#nextBtn").attr("disabled", true);
            urlNext = null;
        }
        else {
            $("#nextBtn").removeAttr("disabled");
        }

    });
    return false;
}
