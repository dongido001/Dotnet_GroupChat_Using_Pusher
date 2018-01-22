let currentGroupId = null;

var pusher = new Pusher('PUSHER_APP_KEY', {
    cluster: 'eu',
    encrypted: true
});

var channel = pusher.subscribe('group_chat');

channel.bind('new_group', function(data) {
    reloadGroup();
});

$("#CreateNewGroupButton").click(function(){
    // get all selected users
    let UserNames = $("input[name='UserName[]']:checked")
                    .map(function(){return $(this).val();}).get();
    
    let data = { 
                GroupName: $("#GroupName").val(),
                UserNames: UserNames
        };
        
    $.ajax({
        type: "POST",
        url: "/api/group",
        data: JSON.stringify(data),
        success: (data) => {
            reloadGroup();
            $('#CreateNewGroup').modal('hide');
        },
        dataType: 'json',
        contentType:'application/json'
    });
    
});

// When a user clicks on a group, Load messages for that particular group.
$(".group").click( function(){
    let group_id = $(this).attr("data-group_id");

    $('.group').css({"border-style": "none", cursor:"pointer"});
    $(this).css({"border-style": "inset", cursor:"default"});

    $("#currentGroup").val(group_id); // update the current group_id to a html form...
    currentGroupId =  group_id;
    
    // get all messages for the group and populate it...
    $.get( "/api/message/"+group_id, function( data ) {
        let message = "";
        
        data.forEach(function(data){
                let position = ( data.addedBy == $("#UserName").val() ) ? " float-right" : "";
                message += `<div class="row chat_message` + position +`"><b>`+ data.addedBy +`: </b>`+ data.message +` </div>`;
        });
        
        $(".chat_body").html(message);
    });
    
   pusher.unsubscribe('private-'+group_id); //unsubscribe

   let group_channel = pusher.subscribe('private-'+group_id);
     
   group_channel.bind('new_message', function(data) { 

        if( currentGroupId == data.new_message.GroupId){

             $(".chat_body").append(`<div class="row chat_message"><b>`+ data.new_message.AddedBy +`: </b>`+ data.new_message.message +` </div>`);
        }
     });  

});

function reloadGroup(){
    $.get("/api/group", function( data ) {
        let groups = "";
               console.log(data); 
       data.forEach(function(group){
           groups += `<div class="group" data-group_id="` +group.groupId+ `">` +group.groupName+  `</div>`;
       });
       
       $("#groups").html(groups);
    });
}

$("#SendMessage").click( function(){
    
    $.ajax({
        type: "POST",
        url: "/api/message",
        data: JSON.stringify( {AddedBy: $("#UserName").val(), GroupId: $("#currentGroup").val(), message: $("#Message").val(), 
        socketId: pusher.connection.socket_id} ),
        success: (data) => { 
            console.log(data);
            $(".chat_body").append(`<div class="row chat_message float-right"><b>`+ data.data.addedBy +`: </b>`+ $("#Message").val() +` </div>`);
            $("#Message").val('');
        },
        dataType: 'json',
        contentType:'application/json'
    });
});