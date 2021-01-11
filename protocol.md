# Multi Part File Upload Protocol
**Version:** 1.0.0 ([SemVer](http://semver.org))<br>
**Date:** 11-01-2021<br>
**Authors:** Eran Yom Tov, Yair Fihman<br>

The key words "MUST", "MUST NOT", "REQUIRED", "SHALL", "SHALL NOT", "SHOULD",
"SHOULD NOT", "RECOMMENDED", "MAY", and "OPTIONAL" in this document are to be
interpreted as described in [RFC 2119](http://www.ietf.org/rfc/rfc2119.txt).

## Status

Following [SemVer](http://semver.org), as of 1.0.0 tus is ready for general
adoption. We don't expect to make breaking changes, but if we do, those will
have to be in a 2.0.0. Introducing a new extension or any backwards-compatible
change adding new functionality will result in a bumped MINOR version.

## Abstract

The protocol provides a mechanism for splited file upload over HTTP protocol.

## Notation

Characters enclosed by square brackets indicate a placeholder (e.g. `[size]`).

The terms space, comma, and semicolon refer to their ASCII representations.

## Core Protocol

The core protocol describes how to get upload link, and start a new upload to it.

All Clients and Servers MUST implement the core protocol.

This specification does not describe the structure of URLs, as that is left for
the specific implementation to decide.  All URLs shown in this document are
meant for example purposes only.

In addition, the implementation of authentication and authorization is left for
the Server to decide.

### Example
The example below shows the basic process of uploading a file in 2 chuncks.

**Request:**

```
Post Method /file
```
```json
{
    "aa":"[aa]",
    ...
    "bb":"[bb]",
    "data":{
        "Compresion":"[gzip/deflta/none]"
    }
}
```

**Response:**

```
HTTP200
Content-Length: [size]
Max-Size: 1024
```
```json
{
    "location": "https://tus.example.org/files/24e533e02ec3bc40c387f1a0e460e216"
}
```

**Reqeust:**
```
PUT /files/17f44dbe1c4bace0e18ab850cf2b3a83 HTTP/1.1
Content-Length: 5
Upload-Offset: 0
Is-Final: 0

hello
```

**Response:**
```
HTTP 204
```

**Reqeust:**
```
PUT /files/17f44dbe1c4bace0e18ab850cf2b3a83 HTTP/1.1
Content-Length: 6
Upload-Offset: 5
Is-Final: 1

-world
```

**Response:**
```
HTTP 204
```

### Headers

#### Upload-Offset

The `Upload-Offset` request and response header indicates a byte offset within a
resource. The value MUST be a non-negative integer.

#### Upload-Length

The `Upload-Length` request and response header indicates the size of the entire
upload in bytes. The value MUST be a non-negative integer. Should be added if known.


#### Max-Size

The `Max-Size` response header MUST be a non-negative integer indicating the maximum
allowed size of an entire upload in bytes. The Server SHOULD set this header if
there is a known hard limit.

#### Is-Final
Sent with the last put requests and indicates the the upload is finished and the file size will be:
</br> ```[Upload-Offset + Content-Length]```

</br>
### Requests

#### PUT

The Server SHOULD accept `PUT` requests against any upload URL and apply the
bytes contained in the message at the given offset specified by the
`Upload-Offset` header. All `PUT` requests MUST use
`Content-Type: application/offset+octet-stream`, otherwise the server SHOULD
return a `415 Unsupported Media Type` status.

The `Upload-Offset` header's value MUST be equal to the current offset of the
resource.

The Client SHOULD send all the remaining bytes of an upload in a single `PUT`
request, but MAY also use multiple small requests successively for scenarios
where this is desirable.

The Server MUST acknowledge successful `PUT` requests with the
`204 No Content` status. It MUST include the `Upload-Offset` header containing
the new offset. The new offset MUST be the sum of the offset before the `PUT`
request and the number of bytes received and processed or stored during the
current `PUT` request.

If the server receives a `PUT` request against a non-existent resource
it SHOULD return a `404 Not Found` status.

Both Client and Server, SHOULD attempt to detect and handle network errors
predictably. They MAY do so by checking for read/write socket errors, as well
as setting read/write timeouts. A timeout SHOULD be handled by closing the underlying connection.

The Server SHOULD always attempt to store as much of the received data as possible.
</br>
</br>
Copyright (c) 2021 Contributors.