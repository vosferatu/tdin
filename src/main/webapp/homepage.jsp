<%@page contentType="text/html" pageEncoding="UTF-8"%>
<!DOCTYPE html>
<html>

<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <title>JSP Page</title>
</head>

<body>
    <jsp:useBean id="user_session" scope="session" class="bookstore.UserSession" />
    <jsp:setProperty name="user_session" property="username" />
    <jsp:setProperty name="user_session" property="email" />

    <h1>Hello
        <jsp:getProperty name="user_session" property="username" />!</h1>

    <h2>Requested Books</h2>

    
    <table>
        <tr>
            <th>Title</th>
            <th>Amount</th>
            <th>State</th>
            <th>Dispatched Date</th>
        </tr>
        <jsp:getProperty name="user_session" property="userBooks" />
    </table>


    <h2> Available Books </h2>
    <table>
        <tr>
            <th>Title</th>
            <th>Price</th>
            <th>Stock</th>
        </tr>
        <jsp:getProperty name="user_session" property="allBooks" />
    </table>


</body>

</html>