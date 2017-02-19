<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    exclude-result-prefixes="msxsl">

    <xsl:output method="xml" />
    <xsl:param name="ApplicationPath" select=" '/' " />

    <xsl:template match=" service ">
        <discovery xmlns="http://schemas.xmlsoap.org/disco/">
            <contractRef xmlns="http://schemas.xmlsoap.org/disco/scl/"
                         ref="{ @url }?wsdl"
                         docRef="{ @url }"/>
        </discovery>
    </xsl:template>

</xsl:stylesheet>
