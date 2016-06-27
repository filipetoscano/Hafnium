<?xml version="1.0" encoding="utf-8"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"
    xmlns:msxsl="urn:schemas-microsoft-com:xslt"
    xmlns:soap="http://schemas.xmlsoap.org/wsdl/soap/"
    xmlns:soap12="http://schemas.xmlsoap.org/wsdl/soap12/"
    xmlns:wsdl="http://schemas.xmlsoap.org/wsdl/"
    xmlns:wsaw="http://www.w3.org/2006/05/addressing/wsdl"
    exclude-result-prefixes="msxsl">

    <xsl:output method="xml" />
    <xsl:param name="ApplicationPath" select=" '/' " />


    <!-- ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    ~
    ~ service/
    ~
    ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ -->
    <xsl:template match=" service ">
        <wsdl:definitions xmlns:wsdl="http://schemas.xmlsoap.org/wsdl/"
                name="{ @name }"
                xmlns:wsam="http://www.w3.org/2007/05/addressing/metadata"
                xmlns:wsx="http://schemas.xmlsoap.org/ws/2004/09/mex"
                xmlns:wsap="http://schemas.xmlsoap.org/ws/2004/08/addressing/policy"
                xmlns:msc="http://schemas.microsoft.com/ws/2005/12/wsdl/contract"
                xmlns:wsp="http://schemas.xmlsoap.org/ws/2004/09/policy"
                xmlns:xsd="http://www.w3.org/2001/XMLSchema"
                xmlns:soap="http://schemas.xmlsoap.org/wsdl/soap/"
                xmlns:wsu="http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"
                xmlns:soap12="http://schemas.xmlsoap.org/wsdl/soap12/"
                xmlns:soapenc="http://schemas.xmlsoap.org/soap/encoding/"
                xmlns:wsa10="http://www.w3.org/2005/08/addressing"
                xmlns:wsaw="http://www.w3.org/2006/05/addressing/wsdl"
                xmlns:wsa="http://schemas.xmlsoap.org/ws/2004/08/addressing">

            <xsl:attribute name="targetNamespace">
                <xsl:value-of select=" @namespace " />
                <xsl:value-of select=" @name " />
            </xsl:attribute>

            <!-- XSLT 2.0 has xsl:namespace, but 1.0 doesn't :( -->
            <xsl:attribute name="tns:dummy" namespace="{ @namespace }{ @name }" />
            <xsl:attribute name="fwk:dummy" namespace="https://github.com/filipetoscano/Zinc" />
            <xsl:attribute name="ns1:dummy" namespace="{ @namespace }" />

            <wsdl:types>
                <xsl:copy-of select=" types/* " />
            </wsdl:types>

            <xsl:apply-templates select=" method " mode="wsdl:message" />

            <wsdl:portType name="I{ @name }">
                <xsl:apply-templates select=" method " mode="wsdl:port-operation" />
            </wsdl:portType>

            <wsdl:binding name="BasicHttpBinding_I{ @name }" type="tns:I{ @name }">
                <soap:binding transport="http://schemas.xmlsoap.org/soap/http"/>
                <xsl:apply-templates select=" method " mode="wsdl:binding-operation" />
            </wsdl:binding>

            <wsdl:service name="{ @name }">
                <wsdl:port name="BasicHttpBinding_I{ @name }" binding="tns:BasicHttpBinding_I{ @name }">
                    <soap:address location="{ @url }" />
                </wsdl:port>
            </wsdl:service>
        </wsdl:definitions>
    </xsl:template>


    <xsl:template match=" method " mode="wsdl:message">
        <wsdl:message name="{ @name }RequestContract">
            <wsdl:part name="{ @name }Request" element="ns1:{ @name }Request" />
        </wsdl:message>
        <wsdl:message name="{ @name }RequestContract_Headers">
            <wsdl:part name="EndpointHeader" element="fwk:EndpointHeader" />
        </wsdl:message>

        <wsdl:message name="{ @name }ResponseContract">
            <wsdl:part name="{ @name }Response" element="ns1:{ @name }Response" />
        </wsdl:message>
        <wsdl:message name="{ @name }ResponseContract_Headers">
            <wsdl:part name="ExecutionHeader" element="fwk:ExecutionHeader" />
        </wsdl:message>

        <wsdl:message name="I{ ../@name }_{ @name }_ActorFaultFault_FaultMessage">
            <wsdl:part name="detail" element="fwk:ActorFault" />
        </wsdl:message>
    </xsl:template>


    <xsl:template match=" method " mode="wsdl:port-operation">
        <wsdl:operation name="{ @name }">
            <wsdl:input wsaw:Action="{ @action }" name="{ @name }RequestContract" message="tns:{ @name }RequestContract" />
            <wsdl:output wsaw:Action="{ @action }Response" name="{ @name }ResponseContract" message="tns:{ @name }ResponseContract" />
            <wsdl:fault wsaw:Action="{ @action }Fault" name="ActorFaultFault" message="tns:I{ ../@name }_{ @name }_ActorFaultFault_FaultMessage" />
        </wsdl:operation>
    </xsl:template>


    <xsl:template match=" method " mode="wsdl:binding-operation">
        <wsdl:operation name="{ @name }">
            <soap:operation soapAction="{ @action }" style="document" />

            <wsdl:input name="{ @name }RequestContract">
                <soap:header message="tns:{ @name }RequestContract_Headers" part="EndpointHeader" use="literal" />
                <soap:body use="literal" />
            </wsdl:input>

            <wsdl:output name="{ @name }ResponseContract">
                <soap:header message="tns:{ @name }ResponseContract_Headers" part="ExecutionHeader" use="literal" />
                <soap:body use="literal" />
            </wsdl:output>

            <wsdl:fault name="ActorFaultFault">
                <soap:fault use="literal" name="ActorFaultFault" namespace="" />
            </wsdl:fault>
        </wsdl:operation>
    </xsl:template>

</xsl:stylesheet>
