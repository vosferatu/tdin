DROP TABLE IF EXISTS Request;
DROP TABLE IF EXISTS Book;
DROP TABLE IF EXISTS SingleOrder;

CREATE TABLE IF NOT EXISTS Book (
    title VARCHAR(128),
    price FLOAT,
    stock INT,
    PRIMARY KEY(title)
);

CREATE TABLE IF NOT EXISTS SingleOrder (
    id BIGINT NOT NULL AUTO_INCREMENT,
    client_name VARCHAR(256) NOT NULL,
    addr VARCHAR(256) NOT NULL,
    email VARCHAR(256) NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS Request (
    amount INT,
    book_title VARCHAR(128),
    order_id BIGINT,
    disp_date DATETIME,
    req_state ENUM('waiting', 'dispatched', 'dispatching'),
    PRIMARY KEY (book_title, order_id),
    FOREIGN KEY (book_title) REFERENCES Book(title) ON DELETE CASCADE,
    FOREIGN KEY (order_id) REFERENCES SingleOrder(id) ON DELETE CASCADE
);

INSERT IGNORE INTO Book SET title='GoT', price=10.5, stock=5;
INSERT IGNORE INTO Book SET title='LoTR', price=9.9, stock=2;
INSERT IGNORE INTO Book SET title='SW', price=6.4, stock=3;
INSERT IGNORE INTO Book SET title='IJ', price=7.9, stock=6;
INSERT IGNORE INTO Book SET title='DBz', price=4.3, stock=1;

INSERT IGNORE INTO SingleOrder SET id=1, client_name='Joao', addr='rua1', email='email1@gmail.com'; 
INSERT IGNORE INTO SingleOrder SET id=2, client_name='Andre', addr='rua2', email='email2@gmail.com';

INSERT IGNORE INTO Request SET amount=1, book_title='GoT', order_id=1, disp_date=NULL, req_state='waiting';
INSERT IGNORE INTO Request SET amount=2, book_title='SW', order_id=1, disp_date=NULL, req_state='waiting';
INSERT IGNORE INTO Request SET amount=4, book_title='IJ', order_id=2, disp_date=NULL, req_state='waiting';