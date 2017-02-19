<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    exclude-result-prefixes="msxsl">

    <xsl:output method="html" />
    <xsl:param name="ApplicationPath" select=" '/' " />


    <xsl:template match=" service ">
        <html>
            <head>
                <meta charset="utf-8" />
                <meta http-equiv="x-ua-compatible" content="ie=edge" />
                <meta name="viewport" content="width=device-width, initial-scale=1" />
                <title>
                    <xsl:value-of select=" @name " />
                </title>

                <link rel="icon" href="{ $ApplicationPath }assets/img/favicon-62.png" sizes="62x62" type="image/png" />
                <link rel="stylesheet" href="{ $ApplicationPath }assets/css/hf.css" />
            </head>
            <body>
                <h1>
                    <xsl:value-of select=" @name " />
                </h1>

                <a href="?wsdl">WSDL</a>

                <ul>
                    <xsl:apply-templates select=" rule ">
                        <xsl:sort select=" @name " />
                    </xsl:apply-templates>
                </ul>
            </body>
        </html>
    </xsl:template>


    <xsl:template match=" rule ">
        <li>
            <div>
                <a href="?op={ @name }">
                    <xsl:value-of select=" @name " />
                </a>

                <xsl:if test=" @publicName != '' ">
                    <xsl:text> - </xsl:text>
                    <xsl:value-of select=" @publicName "/>
                </xsl:if>
            </div>

            <xsl:if test=" @description != '' ">
                <div>
                    <xsl:value-of select=" normalize-space( @description ) " />
                </div>
            </xsl:if>
        </li>
    </xsl:template>

</xsl:stylesheet>
