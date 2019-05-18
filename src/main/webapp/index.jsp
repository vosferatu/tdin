<%@taglib prefix="sql" uri="http://java.sun.com/jsp/jstl/sql"%>
<!DOCTYPE html>
<html>

<head>
    <title>Bookstore Start Page</title>
    <meta http-equiv="Content-Type" content="text/html; charset=UTF-8">
</head>

<body>
    <h1>User Login</h1>

    <form name="UserLogin" action="homepage.jsp">
        <input type="text" name="username" value="" size="100" />
        <input type="text" name="email" value="" size="100" />
        <input type="submit" value="Log in" name="submit" />
        <input type="reset" value="Reset" name="reset" />
    </form>
</body>

</html>