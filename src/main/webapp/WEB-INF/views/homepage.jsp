<%@ taglib prefix="c" uri="http://java.sun.com/jsp/jstl/core" %>

<!DOCTYPE html>
<html>

<head>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
    <title>${username} Homepage</title>
</head>

<body>
    <script>
        function submitRequest() {
            console.log("Submitting");
            var requests = document.getElementsByTagName("request_amount");
            console.log(requests);
        }

        function resetRequestAmounts() {
            console.log("Resetting amounts");
        }
    </script>

    <h1>Hello ${username}!</h1>

    <h2>Requested Books</h2>

    <table>
        <tr>
            <th>Title</th>
            <th>Amount</th>
            <th>State</th>
            <th>Dispatched</th>
        </tr>
        
        <c:forEach var='book' items='${requested_books}'>
            <tr>
                <td><c:out value="${book.title}" /></td>
                <td><c:out value="${book.amount}" /></td>
                <td><c:out value="${book.state}" /></td>
                <td><c:out value="${book.disp_date}" />No Date</td>
            </tr>
        </c:forEach>
    </table>


    <h2> Available Books </h2>
    <form>
        <table>
            <tr>
                <th>Title</th>
                <th>Price</th>
                <th>Stock</th>
                <th>Request</th>
            </tr>
            
            ${all_books}
        </table>
        
        <button onclick="submitRequest();">Request</button>
        <input type="reset" name="Reset Amounts" onclick="resetRequestAmounts();" />
    </form>

</body>
</html>