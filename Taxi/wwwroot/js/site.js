//import { get } from "http";

//import { error } from "util";

const uri = 'api/';
let token = "";

function createCustomer() {
    console.log("create");
    const user = {
        "firstName" : $("#firstName").val(),
        "lastName" : $("#lastName").val(),
        "password": $("#password").val(),
        "email": $("#email").val(),
        "phoneNumber": $("#phone").val()
    }
    const customerUri = uri + "accounts/customers";
    console.log(user);
    $.ajax({
        type: 'POST',
        accepts: 'application/json',
        url: customerUri,
        contentType: 'application/json',
        data: JSON.stringify(user),
        error: function (jqXHR, textStatus, errorThrown) {
            var obj = jqXHR.responseJSON;
            var values = Object.keys(obj).map(function (key) { return obj[key]; });
            $("#response").html(errorThrown + " " + textStatus + "\n" +
                values);
            console.log(errorThrown);
            console.log(textStatus);
            console.log(jqXHR);
        },
        success: function (result) {
            $("#response").html(JSON.stringify(result));
        }
    });
}

function getImg() {
    var image = document.getElementById("image");
    var url = 'http://taxi-env.hsgu7qika6.us-east-2.elasticbeanstalk.com/api/images/3a9c95f0-fbf9-45c9-989f-699e2a649e86.jpg';
    var headers = new Headers({ "Authorization": 'Bearer ' + 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJwYXZsaXZza2l5dm9sb2R5bXlyQGdtYWlsLmNvbSIsImp0aSI6ImFiMzQxMDk5LWU5ZDItNDAyZi1hNDBmLTM2MGJhMWIxYzAxNiIsImlhdCI6MTUzMzkwNzg1NywiaWQiOiIyNzc5ZjUzZC1kNGNmLTRlZDItYmU3MS1jNmJiYzUxMzk5MTciLCJyb2wiOlsiZHJpdmVyX2FjY2VzcyIsImN1c3RvbWVyX2FjY2VzcyJdLCJjdXN0b21lcklkIjoiNTUyNTA5YjctYWNlYS00ZTRkLWEyNWEtYWRkNTI3NWFhY2RhIiwiZHJpdmVySWQiOiIzZDdlMmE4MS1mZDcwLTRhMGUtOTY4Ni1jOWQyN2FkM2M4NDciLCJuYmYiOjE1MzM5MDc4NTcsImV4cCI6MTUzMzkxNTA1NywiaXNzIjoid2ViQXBpIiwiYXVkIjoiaHR0cDovL2xvY2FsaG9zdDo1Mjg0MS8ifQ.rjfGhggydlDSdpIXItYTyjt0Q9vs1S59eZXL1hfiAzA' });
    var options = {
        method: 'GET',
        headers: headers,
        mode: 'cors',
        cache: 'default'
    };
    var request = new Request(url);

    fetch(request, options).then((response) =>

        response.body)


            .then(stream => new Response(stream))
            .then(response => response.blob())
            .then(blob => URL.createObjectURL(blob))
            .then(url => console.log(image.src = url))
            .catch(err => console.error(err));
        
}

function createDriver() {
    console.log("createDriver");
    const user = {
        "firstName": $("#firstNameDriver").val(),
        "lastName": $("#lastNameDriver").val(),
        "password": $("#passwordDriver").val(),
        "email": $("#emailDriver").val(),
        "phoneNumber": $("#phoneDriver").val(),
        "city" : $("#city").val()
    }
    const customerUri = uri + "accounts/drivers";
    console.log(user);
    $.ajax({
        type: 'POST',
        accepts: 'application/json',
        url: customerUri,
        contentType: 'application/json',
        data: JSON.stringify(user),
        error: function (jqXHR, textStatus, errorThrown) {
            var obj = jqXHR.responseJSON;
            var values = Object.keys(obj).map(function (key) { return obj[key]; });
            $("#response").html(errorThrown + " " + textStatus + "\n" +
                values);console.log(errorThrown);
            console.log(jqXHR);
        },
        success: function (result) {
            $("#response").html(JSON.stringify(result));
            console.log(result);
        }
    });
}

function login() {
    console.log("login");
    const credencials = {
        "userName": $("#emailLogin").val(),
        "password": $("#passwordLogin").val()
    }
    const customerUri = uri + "auth/login";

    $.ajax({
        type: 'POST',
        accepts: 'application/json',
        url: customerUri,
        contentType: 'application/json',
        data: JSON.stringify(credencials),
        error: function (jqXHR, textStatus, errorThrown) {
            var obj = jqXHR.responseJSON;
            var values = Object.keys(obj).map(function (key) { return obj[key]; });
            $("#response").html(errorThrown + " " + textStatus + "\n" +
                values);console.log(errorThrown);
            console.log(textStatus);
        },
        success: function (result) {
            $("#response").html("login succeed");
         
            token = result.auth_token;
            console.log(token);
        }
    });

}


function testGet() {
    const customerUri = uri + "auth";
    $.ajax({
        type: 'GET',
        accepts: 'application/json',
        url: customerUri,
        headers: { "Authorization": 'Bearer ' + token },
        contentType: 'application/json',
        error: function (jqXHR, textStatus, errorThrown) {
            $("#response").html(errorThrown + " " + textStatus);
            console.log(errorThrown);
            console.log(textStatus);
        },
        success: function (result) {
            $("#response").html(result);
        }
    });
}
