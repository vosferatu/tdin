<!DOCTYPE html>
<html>

<head>
    <title>Bookstore Start Page</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
</head>

<body>
    <h1>User Login</h1>

    <c:if test="${violations != null}">
        <c:forEach items="${violations}" var="violation">
            <p>${violation}</p>
        </c:forEach>
    </c:if>

    <form action="${pageContext.request.contextPath}/login" method="post">
        <label for="firstname">Client Name:</label>
        <br>
        <input type="text" name="username" id="username" value="${username}">
        <br>
        <label for="email">Address: </label>
        <br>
        <input type="text" name="address" id="address" value="${address}">
        <br>
        <label for="email">Email: </label>
        <br>
        <input type="text" name="email" id="email" value="${email}">
        <br>
        <input type="submit" name="signup" value="Sign Up">
        <input type="reset" value="Reset" name="reset" />
    </form>
</body>

</html>
