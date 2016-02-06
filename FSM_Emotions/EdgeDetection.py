from naoqi import ALProxy
from naoqi import ALModule
import vision_definitions
import time
import random

import numpy as np
import cv2
import math


class EdgeDetection:
    def __init__(self,ip, port):
        self.NAOip = ip
        self.NAOport = port
        self.camProxy = ALProxy("ALVideoDevice", self.NAOip, self.NAOport)
        self.resolution = vision_definitions.kQVGA
        self.colorSpace = vision_definitions.kBGRColorSpace
        self.fps = 10
        self.cameraId = self.camProxy.subscribe("python_GVM", self.resolution, self.colorSpace, self.fps)
        print "The camera ID is", self.cameraId
        self.camProxy.setActiveCamera(self.cameraId, vision_definitions.kBottomCamera)
        self.bDebug = False
        self.kernelClosing = np.ones((5, 5), np.uint8)
        self.kernelDilate = np.ones((5,5), np.uint8)
        self.kernelErode = np.ones((5,5), np.uint8)
        self.voteHoughLines=50


    def __del__(self):
        self.camProxy.unsubscribe(self.cameraId)
        if self.bDebug:
            print "End camera subscription"

    def Point2PointDistance(self,x1,y1,x2,y2):
        return math.sqrt((x2-x1)*(x2-x1)+(y2-y1)*(y2-y1))

    def Vector2VectorAngle(self,xs1,ys1,xe1,ye1,xs2,ys2,xe2,ye2):
        cos = self.dotProduct(xs1,ys1,xe1,ye1,xs2,ys2,xe2,ye2)/(self.Point2PointDistance(xs1,ys1,xe1,ye1)*self.Point2PointDistance(xs2,ys2,xe2,ye2))
        return math.acos(cos)

    def dotProduct(self,xs1,ys1,xe1,ye1,xs2,ys2,xe2,ye2):
        return (xe1-xs1)*(xe2-xs2)+(ye1-ys1)*(ye2-ys2)

    def ChooseValidTablePoint(self,x1,y1,x2,y2,xt,yt,xi,yi):
        if self.dotProduct(x1,y1,xi,yi,x2,y2,xi,yi)< 0:
            p1 = self.dotProduct(x1,y1,xi,yi,xt,yt,xi,yi)
            p2 = self.dotProduct(x2,y2,xi,yi,xt,yt,xi,yi)
            if p1 < 0:
                return x1,y1
            if p2 < 0:
                return x2,y2
        else:
            if self.Point2PointDistance(x1,y1,xi,yi) > self.Point2PointDistance(x2,y2,xi,yi):
                return x2,y2
            else:
                return x1,y1
        return -1,-1

    def EdgeDistance2D(self,x1, y1, x2, y2):
        a = y1-y2
        b = x2-x1
        c = y2*(x1-x2)-x2*(y1-y2)
        return abs(a*self.centerX+b*self.centerY+c)/math.sqrt(a*a+b*b)

    def EdgeDistance3D(self,dist2d):
        distFeetToCenter = 292.1
        dist3d = distFeetToCenter/(self.maxY/2)*dist2d
        return dist3d

    def ShortestIntersection(self,x1,y1,x2,y2):
        a = y1-y2
        b = x2-x1
        c = y2*(x1-x2)-x2*(y1-y2)
        m = a*self.centerY-b*self.centerX
        den = a*a+b*b
        x = (-b*m-c*a)/den
        y = (a*m-b*c)/den
        return x,y

    def RhoThetaX1Y1X2Y2(self,rho,theta):
        a = np.cos(theta)
        b = np.sin(theta)
        x0 = a*rho
        y0 = b*rho
        x1 = int(x0 + 1000*(-b))
        y1 = int(y0 + 1000*(a))
        x2 = int(x0 - 1000*(-b))
        y2 = int(y0 - 1000*(a))
        return x1,y1,x2,y2

    def RhoThetaToBoundary(self,rho, theta):
        xTop = -1
        yLeft = -1
        xBottom = -1
        yRight = -1
        if math.sin(theta)<0.001:
            if rho>0:
                xTop = rho
                xBottom = rho
            elif rho<0:
                if self.bDebug==True:
                    print "Error 07: Impossible Line! Press any key to continue"
                    print rho,theta
                    print self.RhoThetaX1Y1X2Y2(rho,theta)
                    #x1,y1,x2,y2 = self.RhoThetaX1Y1X2Y2(rho,theta)
                    #cv2.line(houghImage, (x1, y1), (x2, y2), (0, 0, 0xFF), 2)
                    #cv2.imshow('Hough Transform', houghImage)
                    #cv2.waitKey(0)
        elif theta<(np.pi/2):
            if rho>0:
                yLeft=rho/math.sin(theta)
                if yLeft <= self.maxY:
                    xBottom = -1
                else:
                    xBottom= (yLeft-self.maxY)*math.tan(theta)
                    yLeft = -1
                xTop = rho/math.cos(theta)
                if xTop < self.maxX:
                    yRight = -1
                else:
                    yRight = (xTop-self.maxX)/math.tan(theta)
                    xTop = -1
            elif rho < 0:
                if self.bDebug==True:
                    print "Error 01: Impossible case. Press any key to continue"
                    print rho,theta
                    print self.RhoThetaX1Y1X2Y2(rho,theta)
                    #x1,y1,x2,y2 = self.RhoThetaX1Y1X2Y2(rho,theta)
                    #cv2.line(houghImage, (x1, y1), (x2, y2), (0, 0xFF, 0), 2)
                    #cv2.imshow('Hough Transform', houghImage)
                    #cv2.waitKey(0)
            elif rho==0:
                if self.bDebug==True:
                    print "Error 02: Won't show. Ignored!"
                    print rho,theta
                    print self.RhoThetaX1Y1X2Y2(rho,theta)
                    #x1,y1,x2,y2 = RhoThetaX1Y1X2Y2(rho,theta)
                    #cv2.line(houghImage, (x1, y1), (x2, y2), (0, 0xFF, 0), 2)
                    #cv2.imshow('Hough Transform', houghImage)
        elif theta>(np.pi/2) and theta<(np.pi):
            if rho>0:
                yLeft=rho/math.cos(theta-np.pi/2)
                if yLeft > self.maxY:
                    yLeft = -1
                    if self.bDebug == True:
                        print "Error 03: Unexpected Line! Press any key to continue"
                        print rho,theta
                        print self.RhoThetaX1Y1X2Y2(rho,theta)
                        #x1,y1,x2,y2 = RhoThetaX1Y1X2Y2(rho,theta)
                        #cv2.line(houghImage, (x1, y1), (x2, y2), (0, 0xFF, 0), 2)
                        #cv2.imshow('Hough Transform', houghImage)
                        #cv2.waitKey(0)
                else:
                    xBottom = (self.maxY - yLeft)/math.tan(theta-np.pi/2)
                    if xBottom > self.maxX:
                        yRight = self.maxY - (xBottom-self.maxX)*math.tan(theta-np.pi/2)
                        xBottom= - 1
            elif rho < 0:
                xTop = -rho/math.sin(theta-np.pi/2)
                if xTop > self.maxX:
                    if self.bDebug==True:
                        print "Error 04: Unexpected Line! Press any key to continue"
                        print rho,theta
                        print self.RhoThetaX1Y1X2Y2(rho,theta)
                        #x1,y1,x2,y2 = RhoThetaX1Y1X2Y2(rho,theta)
                        #cv2.line(houghImage, (x1, y1), (x2, y2), (0, 0xFF, 0), 2)
                        #cv2.imshow('Hough Transform', houghImage)
                        #cv2.waitKey(0)
                else:
                    yRight=(self.maxX-xTop)*math.tan(theta-np.pi/2)
                    if yRight > self.maxY:
                        xBottom = self.maxX - (yRight-self.maxY)/math.tan(theta-np.pi/2)
                        yRight = -1
            elif rho == 0:
                yLeft = 1;
                yRight = self.maxX*math.tan(theta-np.pi/2)
                if yRight > self.maxY:
                    xBottom = (yRight - self.maxY)/math.tan(theta-np.pi/2)
                    yRight = -1
        elif theta>np.pi:
            if self.bDebug == True:
                print "Error 05: Unexpected Angle! Press any key to continue"
                print rho,theta
                print self.RhoThetaX1Y1X2Y2(rho,theta)
                #x1,y1,x2,y2 = RhoThetaX1Y1X2Y2(rho,theta)
                #cv2.line(houghImage, (x1, y1), (x2, y2), (0, 0xFF, 0), 2)
                #cv2.imshow('Hough Transform', houghImage)
                #cv2.waitKey(0)
        if xTop > self.maxX or xBottom > self.maxX or yLeft> self.maxY or yRight> self.maxY:
            if self.bDebug == True:
                print "Error 06: Out of Range! Press any key to continue"
                print rho,theta
                print self.RhoThetaX1Y1X2Y2(rho,theta)
                #x1,y1,x2,y2 = RhoThetaX1Y1X2Y2(rho,theta)
                #cv2.line(houghImage, (x1, y1), (x2, y2), (0, 0xFF, 0), 2)
                #cv2.imshow('Hough Transform', houghImage)
                #cv2.waitKey(0)
        return xTop,yLeft,xBottom,yRight

    def ProcessFrame(self,originalFrame):

        #ret, originalFrame = cap.read()
        #if ret == False:
        #    break
        #maxX = originalFrame.shape[1]#320
        #maxY = originalFrame.shape[0]#240

        compactMax=400
        numberOfLines = 0
        Rho1=0
        Theta1=0
        Rho2=1
        Theta2=1
        distance3D=-1
        angleMajor=-1
        angleMinor=-1

        blur_bgr = cv2.bilateralFilter(originalFrame, 15, 75, 75)
        edge_bgr = cv2.Canny(blur_bgr, 10, 200, True)
        closing = cv2.morphologyEx(edge_bgr, cv2.MORPH_CLOSE,  self.kernelClosing)
        replicate_bgr = cv2.copyMakeBorder(closing,1,1,1,1,cv2.BORDER_REPLICATE)
        cv2.floodFill(blur_bgr,replicate_bgr,(self.seedX,self.seedY),(0,0xFF,0), (5,10,10), (5,10,10), 4 | ( 125 << 8 ) | cv2.FLOODFILL_MASK_ONLY )
        FloodFill = cv2.inRange(replicate_bgr,125,125)
        TableRegion = cv2.dilate(FloodFill,  self.kernelDilate, iterations=1)
        #TableRegion = cv2.erode(TableRegion,  self.kernelErode, iterations=2)
        TableEdges = cv2.Canny(TableRegion, 1, 255, apertureSize=5)
        houghImage = np.zeros(( self.maxY,  self.maxX, 3), np.uint8)
        #coutourFrame = np.zeros(( self.maxY,  self.maxX, 3), np.uint8)
        lines = cv2.HoughLines(TableEdges,1,np.pi/180, self.voteHoughLines)


        #if self.bDebug:
            #cv2.imshow('Original', originalFrame)
            #cv2.imshow('Edge',replicate_bgr)
            #cv2.imshow('Smooth',blur_bgr)
            #cv2.imshow('Closing',closing)
            #cv2.imshow('TableEdge',TableEdges)
            #cv2.imshow('TableRegion',TableRegion)
            #cv2.imshow('FloodFill',FloodFill)
            #cv2.waitKey(1)


        if lines is None:
            if self.bDebug:
                print "No line detected"
            self.voteHoughLines-=3;
            if  self.voteHoughLines<=0:
                self.voteHoughLines=20
            #bBadDetection=True
        else:
            for rho,theta in lines[0]:
                x1,y1,x2,y2= self.RhoThetaX1Y1X2Y2(rho,theta)
                cv2.line(houghImage, (x1, y1), (x2, y2), (0, 0, 0xFF), 2)
            #if  self.bDebug:
            #    cv2.imshow('Hough Transform', houghImage)
            #    cv2.waitKey(1)
            lineArrayDimension = lines.shape

            if lineArrayDimension[1]==1:
                self.voteHoughLines-=7
            elif lineArrayDimension[1]==2:
                self.voteHoughLines-=3
            if  self.voteHoughLines<=0:
                self.voteHoughLines=20
            if lineArrayDimension[1]>2:
                criteria = (cv2.TERM_CRITERIA_EPS + cv2.TERM_CRITERIA_MAX_ITER, 10, 1.0)
                flags = cv2.KMEANS_RANDOM_CENTERS
                compactness,labels,centers = cv2.kmeans(lines,2,criteria,10,flags)
                if compactness>compactMax:
                    if  self.bDebug:
                        print "kmean fails to converge", compactness
                        #bBadDetection=True
                    self.voteHoughLines+=3
                elif lineArrayDimension[1]>7:
                     self.voteHoughLines+=3
                elif lineArrayDimension[1]>100:
                    if  self.bDebug:
                        print "Too many lines, continue:",lineArrayDimension, self.voteHoughLines
                    #bBadDetection=True
                    voteHoughLines=50
                Diff=abs(centers[0][1]-centers[1][1])
                if Diff>np.pi/2:
                    Diff=np.pi-Diff
                if Diff>np.pi/4:
                    Rho1=centers[0][0]
                    Theta1=centers[0][1]
                    Rho2=centers[1][0]
                    Theta2=centers[1][1]
                    numberOfLines=2
                else:
                    Rho1=(lines[0][0][0]+lines[0][1][0])*0.5
                    Theta1=(lines[0][0][1]+lines[0][1][1])*0.5
                    numberOfLines=1
            elif lineArrayDimension[1] == 2:
                Diff=abs(lines[0][0][1]-lines[0][1][1])
                if Diff>np.pi/2:
                    Diff=np.pi-Diff
                if Diff > np.pi/4:
                    Rho1=lines[0][0][0]
                    Theta1=lines[0][0][1]
                    Rho2=lines[0][1][0]
                    Theta2=lines[0][1][1]
                    numberOfLines=2
                else:
                    ho1=(lines[0][0][0]+lines[0][1][0])*0.5
                    Theta1=(lines[0][0][1]+lines[0][1][1])*0.5
                    numberOfLines=1
            elif lineArrayDimension[1] == 1:
                Rho1=lines[0][0][0]
                Theta1=lines[0][0][1]
                numberOfLines=1

        if numberOfLines==1:
            xTop,yLeft,xBottom,yRight =  self.RhoThetaToBoundary(Rho1, Theta1)
            x1,y1,x2,y2 = self.RhoThetaX1Y1X2Y2(Rho1, Theta1)
            x,y = self.ShortestIntersection(float(x1),float(y1),float(x2),float(y2))
            distance = self.EdgeDistance2D(x1, y1, x2, y2)
            distance3D = self.EdgeDistance3D(distance)
            angleMajor = self.Vector2VectorAngle(self.centerX,self.centerY,x,y,self.centerX,self.centerY, self.maxX/2,0)
            angleMinor =-1
            cv2.circle(originalFrame, (int(x), int(y)), 5, (0xFF, 0, 0), -1)
            cv2.line(originalFrame, (x1, y1), (x2, y2), (0, 0xFF, 0), 2)
            cv2.line(originalFrame, (int(x), int(y)), (int(self.centerX), int(self.centerY)), (0, 0, 0xFF), 1)
            if xTop >= 0:
                cv2.circle(originalFrame, (int(xTop), 0), 5, (0xFF, 0, 0), -1)
            if yLeft >= 0:
                cv2.circle(originalFrame, (0, int(yLeft)), 5, (0xFF, 0, 0), -1)
            if xBottom >= 0:
                cv2.circle(originalFrame, (int(xBottom),  self.maxY), 5, (0xFF, 0, 0), -1)
            if yRight >= 0:
                cv2.circle(originalFrame, ( self.maxX, int(yRight)), 5, (0xFF, 0, 0), -1)
        elif numberOfLines==2:
            line1Pt=[]
            line2Pt=[]
            xTop1, yLeft1, xBottom1, yRight1 =  self.RhoThetaToBoundary(Rho1, Theta1)
            xTop2, yLeft2, xBottom2, yRight2 =  self.RhoThetaToBoundary(Rho2, Theta2)
            if xTop1>=0:
                line1Pt.append([xTop1, 0])
            if xTop2>=0:
                line2Pt.append([xTop2, 0])
            if yLeft1 >= 0:
                line1Pt.append([0, yLeft1])
            if yLeft2 >= 0:
                line2Pt.append([0, yLeft2])
            if xBottom1 >= 0:
                line1Pt.append([xBottom1,  self.maxY])
            if xBottom2 >= 0:
                line2Pt.append([xBottom2,  self.maxY])
            if yRight1 >= 0:
                line1Pt.append([ self.maxX, yRight1])
            if yRight2 >= 0:
                line2Pt.append([ self.maxX, yRight2])
            if len(line1Pt)==2 and len(line2Pt)==2:
                xIntersectNum = -line2Pt[1][0]*line2Pt[0][1]*line1Pt[0][0]\
                                +line2Pt[1][0]*line2Pt[0][1]*line1Pt[1][0]\
                                +line2Pt[0][0]*line1Pt[1][0]*line1Pt[0][1]\
                                -line2Pt[1][0]*line1Pt[1][0]*line1Pt[0][1]\
                                +line2Pt[1][0]*line1Pt[1][1]*line1Pt[0][0]\
                                -line2Pt[0][0]*line2Pt[1][1]*line1Pt[1][0]\
                                +line2Pt[0][0]*line2Pt[1][1]*line1Pt[0][0]\
                                -line2Pt[0][0]*line1Pt[1][1]*line1Pt[0][0]
                Denum = -line2Pt[0][1]*line1Pt[0][0]\
                        +line2Pt[0][1]*line1Pt[1][0]\
                        +line2Pt[1][1]*line1Pt[0][0]\
                        -line2Pt[1][1]*line1Pt[1][0]\
                        +line1Pt[0][1]*line2Pt[0][0]\
                        -line1Pt[0][1]*line2Pt[1][0]\
                        -line1Pt[1][1]*line2Pt[0][0]\
                        +line1Pt[1][1]*line2Pt[1][0]
                yIntersectNum = -line1Pt[1][1]*line2Pt[1][1]*line2Pt[0][0]\
                                -line2Pt[0][1]*line1Pt[1][1]*line1Pt[0][0]\
                                +line2Pt[0][1]*line1Pt[1][0]*line1Pt[0][1]\
                                +line2Pt[1][1]*line1Pt[1][1]*line1Pt[0][0]\
                                -line2Pt[1][1]*line1Pt[1][0]*line1Pt[0][1]\
                                +line1Pt[0][1]*line2Pt[1][1]*line2Pt[0][0]\
                                -line1Pt[0][1]*line2Pt[1][0]*line2Pt[0][1]\
                                +line1Pt[1][1]*line2Pt[1][0]*line2Pt[0][1]
                xTableCorner = xIntersectNum / Denum
                yTableCorner = yIntersectNum / Denum
                cv2.circle(originalFrame, (int(xTableCorner), int(yTableCorner)), 10, (0xFF, 0xFF, 0), -1)
                x1,y1 =  self.ShortestIntersection(line1Pt[0][0], line1Pt[0][1], xTableCorner, yTableCorner)
                x2,y2 =  self.ShortestIntersection(line2Pt[0][0], line2Pt[0][1], xTableCorner, yTableCorner)
                distance1 = self.EdgeDistance2D(line1Pt[0][0], line1Pt[0][1], xTableCorner, yTableCorner)
                distance2 = self.EdgeDistance2D(line2Pt[0][0], line2Pt[0][1], xTableCorner, yTableCorner)
                if distance1 < distance2:
                    distance3D =  self.EdgeDistance3D(distance1)
                    cv2.circle(originalFrame, (int(x1), int(y1)), 5, (0xFF, 0, 0), -1)
                    cv2.line(originalFrame, (int(x1), int(y1)), (int(self.centerX), int(self.centerY)), (0, 0, 0xFF), 1)
                    cv2.line(originalFrame, (int(x2), int(y2)), (int(self.centerX), int(self.centerY)), (0, 0xFF, 0xFF), 1)
                    angleMajor = self.Vector2VectorAngle(self.centerX,self.centerY,x1,y1,self.centerX,self.centerY, self.maxX/2,0)
                    angleMinor = self.Vector2VectorAngle(self.centerX,self.centerY,x2,y2,self.centerX,self.centerY, self.maxX/2,0)
                else:
                    distance3D = self.EdgeDistance3D(distance2)
                    cv2.circle(originalFrame, (int(x2), int(y2)), 5, (0xFF, 0, 0), -1)
                    cv2.line(originalFrame, (int(x1), int(y1)), (int(self.centerX), int(self.centerY)), (0, 0xFF, 0xFF), 1)
                    cv2.line(originalFrame, (int(x2), int(y2)), (int(self.centerX), int(self.centerY)), (0, 0, 0xFF), 1)
                    angleMajor= self.Vector2VectorAngle(self.centerX,self.centerY,x2,y2,self.centerX,self.centerY, self.maxX/2,0)
                    angleMinor= self.Vector2VectorAngle(self.centerX,self.centerY,x1,y1,self.centerX,self.centerY, self.maxX/2,0)
                if len(line1Pt)>=2:
                    x,y =  self.ChooseValidTablePoint(line1Pt[0][0], line1Pt[0][1], line1Pt[1][0], line1Pt[1][1], xTableCorner, yTableCorner, x1, y1)
                    cv2.circle(originalFrame, (int(x), int(y)), 5, (0xFF, 0, 0), -1)
                    cv2.line(originalFrame, (int(x), int(y)), (int(xTableCorner), int(yTableCorner)), (0, 255, 0), 2)
                if len(line2Pt)>=2:
                    x,y =  self.ChooseValidTablePoint(line2Pt[0][0], line2Pt[0][1], line2Pt[1][0], line2Pt[1][1], xTableCorner, yTableCorner, x2, y2)
                    cv2.circle(originalFrame, (int(x), int(y)), 5, (0xFF, 0, 0), -1)
                    cv2.line(originalFrame, (int(x), int(y)), (int(xTableCorner), int(yTableCorner)), (0, 255, 0), 2)
            else:
                if  self.bDebug:
                    print "Detected two lines, but fail to calculate intersection with border."
                    print Rho1, Theta1
                    print xTop1, yLeft1, xBottom1, yRight1
                    print Rho2, Theta2
                    print xTop2, yLeft2, xBottom2, yRight2
                    cv2.waitKey(0)

        font = cv2.FONT_HERSHEY_SIMPLEX
        if distance3D<0:
            dist3dStr = "N/A"
        else:
            dist3dStr=str(int(distance3D/10))
        if angleMajor<0:
            angleMajorStr = "N/A"
        else:
            angleMajorStr=str(int(np.degrees(angleMajor)))
        if angleMinor<0:
            angleMinorStr = "N/A"
        else:
            angleMinorStr=str(int(np.degrees(angleMinor)))

        if self.bDebug:
            cv2.putText(originalFrame,"Distance:" + dist3dStr + " cm",(10, self.maxY-10), font, 1,(0,0xFF,0),2,cv2.CV_AA)
            cv2.putText(originalFrame,"Major Angle:" + angleMajorStr + " deg",(10,25), font, 0.75,(0,0,0xFF),1,cv2.CV_AA)
            cv2.putText(originalFrame,"Minor Angle:" + angleMinorStr + " deg",(10,55), font, 0.75,(0,0xFF,0xFF),1,cv2.CV_AA)
            cv2.circle(originalFrame, (self.seedX, self.seedY), 3, (0xFF, 0, 0), -1)
            cv2.imshow('Output', originalFrame)
            cv2.waitKey(1)

        return distance3D/10,angleMajor #distance in cm, angle in degree


    def SetDebugMode(self,bMode):
         self.bDebug = bMode

    def GetFrame(self):
        naoImage = self.camProxy.getImageRemote(self.cameraId)
        frame = (np.reshape(np.frombuffer(naoImage[6], dtype = '%iuint8' % naoImage[2]), (naoImage[1], naoImage[0], naoImage[2])))
        return naoImage,frame

    def UpdateParameters(self,width,height):
        self.maxX=width
        self.maxY=height
        self.seedX = int(self.maxX/2)
        self.seedY = int(self.maxY*0.95)
        self.centerX = int(self.maxX/2)
        self.centerY = int(self.maxY)

    def lookForEdge(self,Threshold,FrameToTake=None):
        #NOTE: distance in cm, angle in degree

        if FrameToTake is None:
            totalFrame = 5
        else:
            totalFrame = FrameToTake
        countSuccess=0
        distanceList = []
        angleList = []
        for i in xrange(totalFrame):
            if self.bDebug:
                print "Analyzing ",i," frame"
            naoImage, originalFrame = self.GetFrame()
            self.UpdateParameters(naoImage[0],naoImage[1])
            distance, angle = self.ProcessFrame(originalFrame)

            if distance >= 0 and angle >=0:
                distanceList.append(distance)
                angleList.append(angle)
                countSuccess+=1
        if self.bDebug:
            cv2.destroyAllWindows()
        if countSuccess > (totalFrame*0.7):
            distance = np.mean(distanceList)
            angle = np.mean(angleList)
            if distance > Threshold:
                return False, distance, angle
            else:
                return True, distance, angle
        else:
            return False, -1, -1

    '''
    def connectToProxy(self, proxyName):
        try:
            proxy = ALProxy(proxyName, self.NAOip, self.NAOport)
        except Exception, e:
            print "Could not create Proxy to ", proxyName
            print "Error was: ", e
        return proxy
    '''
    '''
    def update(self, names, keys, times):
        postureProxy = self.connectToProxy("ALRobotPosture")
        standResult = postureProxy.goToPosture("StandInit", 0.3)
        if (standResult):
            print("------> Stood Up")
            try:
                # uncomment the following line and modify the IP if you use this script outside Choregraphe.
                print("Time duration is ")
                print(max(max(times)))
                motionProxy = self.connectToProxy("ALMotion")
                motionProxy.angleInterpolation(names, keys, times, True)
                print 'Tasklist: ', motionProxy.getTaskList();
                time.sleep(max(max(times))+0.5)
            except BaseException, err:
                print err
        else:
            print("------> Did NOT Stand Up...")
    '''
    '''
    def happyEmotion(self):
        names = list()
        times = list()
        keys = list()
        names.append("HeadPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.0138481, -0.0138481, -0.50933, 0.00762796])

        names.append("HeadYaw")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.00924587, -0.00924587, -0.0138481, -0.0138481])

        names.append("LAnklePitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.345192, -0.357464, -0.354396, -0.354396])

        names.append("LAnkleRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.00157595, 0.00157595, 0.00157595, 0.00157595])

        names.append("LElbowRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.99399, -0.99399, -0.983252, -0.99399])

        names.append("LElbowYaw")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-1.37297, -1.37297, -1.37297, -1.37297])

        names.append("LHand")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.2572, 0.2572, 0.2572, 0.2572])

        names.append("LHipPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.447886, -0.447886, -0.447886, -0.447886])

        names.append("LHipRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([4.19617e-05, 4.19617e-05, 4.19617e-05, 4.19617e-05])

        names.append("LHipYawPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.0061779, -0.00455999, -0.00455999, -0.00455999])

        names.append("LKneePitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.70253, 0.70253, 0.70253, 0.70253])

        names.append("LShoulderPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([1.4097, 1.4097, 1.42044, 1.4097])

        names.append("LShoulderRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.291418, 0.291418, 0.28068, 0.291418])

        names.append("LWristYaw")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.0123138, -0.0123138, -0.0123138, -0.0123138])

        names.append("RAnklePitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.34971, -0.34971, -0.34971, -0.34971])

        names.append("RAnkleRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.00609398, 0.00464392, 0.00464392, 0.00464392])

        names.append("RElbowRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([1.01555, 1.43893, 0.265424, 1.53251])

        names.append("RElbowYaw")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([1.3913, 1.64287, 1.61679, 1.35755])

        names.append("RHand")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.2544, 0.2544, 0.9912, 0.0108])

        names.append("RHipPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.45564, -0.45564, -0.444902, -0.444902])

        names.append("RHipRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.00924587, -0.00149202, -0.00149202, -0.00149202])

        names.append("RHipYawPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.0061779, -0.00455999, -0.00455999, -0.00455999])

        names.append("RKneePitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.70108, 0.70108, 0.70108, 0.70108])

        names.append("RShoulderPitch")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([1.41132, 0.535408, -1.0216, 0.842208])

        names.append("RShoulderRoll")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([-0.259288, 0.032172, 0.0444441, 0.202446])

        names.append("RWristYaw")
        times.append([0.8, 1.56, 2.12, 2.72])
        keys.append([0.026036, 1.63213, 1.63213, 1.63213])
        self.update(names, keys, times)
        '''