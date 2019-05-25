<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<!DOCTYPE html>
<html>

<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <title>${username} Homepage</title>
    <script type="text/javascript" src="/bookstore/scripts/homepage.js"></script>
    <script src="https://ajax.googleapis.com/ajax/libs/jquery/3.4.0/jquery.min.js"></script>
</head>

<body>
    <script>
        setUserInfos('${username}', '${address}', '${email}'); 
    </script>
    <h1>Hello ${username}!</h1>

    <h2>Requested Books</h2>

    <c:forEach var='request' items='${requests}'>
        <h3>Order #<c:out value='${request.ID}'/></h3>
        <table>
            <tr>
                <th>Title</th>
                <th>Amount</th>
                <th>State</th>
                <th>Dispatched</th>
            </tr>
            
            <c:forEach var='book' items='${request.requestBooks}'>
                <tr>
                    <td><c:out value="${book.title}" /></td>
                    <td><c:out value="${book.amount}" /></td>
                    <td><c:out value="${book.state}" /></td>
                    <td><c:out value="${book.disp_date}">No Date</c:out></td>
                </tr>
            </c:forEach>
        </table>
    </c:forEach>

    <br>
    <h2> Available Books </h2>
        <table>
            <tr>
                <th>Title</th>
                <th>Price</th>
                <th>Amount</th>
                
            </tr>
            <c:forEach var='book' items='${all_books}'>
                <tr>
                    <td><c:out value='${book.title}' /></td>
                    <td><c:out value='${book.price}' /></td>
                    <td class="BookAmount" id="${book.title}">0</td>
                    <td><button onclick='addBook("${book.title}");'>+</button></td>
                    <td><button onclick='removeBook("${book.title}");'>-</button></td>
                </tr>
            </c:forEach>
        </table>

        <button onclick="submitRequest();">Submit</button>
        <button onclick="resetRequest();">Reset</button>

</body>
</html>