import time
import threading
import Tkinter as tk
import tkMessageBox
import sys

class ThreadedChecker(threading.Thread):
    def __init__(self, threadID, name, genUtil):
        threading.Thread.__init__(self)

        self.threadID = threadID
        self.name = name

        self.genUtil = genUtil

    def run(self):
        print "Starting " + self.name
        self.thread_main()
        print "Ending " + self.name

    def thread_main(self):
        self.root = tk.Tk()
        w = 200
        h = 200
        ws = self.root.winfo_screenwidth()
        hs = self.root.winfo_screenheight()
        self.root.geometry('%dx%d+%d+%d' % (w, h, ws-w-25, 10))
        b = tk.Button(self.root, text="Freak out", command = self.freakOutClicked)
        b.pack()
        b2 = tk.Button(self.root, text="Freak out Stop", command = self.freakOutEndClicked)
        b2.pack()

        self.root.mainloop()

    def freakOutClicked(self):
        print
        print "HOOOOOOLLLLLYYYYYY CRRRRAAAAAAAP"
        print
        # tkMessageBox.showinfo("Freaking Out", "HOLY CRAP")
        self.genUtil.naoIsSafe = False
        self.genUtil.stopNAOActions()

    def freakOutEndClicked(self):
        print "MUCH BETTER"
        # tkMessageBox.showinfo("Freaking Out", "HOLY CRAP")
        self.genUtil.naoIsSafe = True
        # self.genUtil.naoStand()

    def quit(self):
        self.root.quit()

#
#import threading, msvcrt
#import sys
#
#def readInputTimeout(caption, default, timeout = 5):
#    class KeyboardThread(threading.Thread):
#        def run(self):
#            self.timedout = False
#            self.input = ''
#            while True:
#                if msvcrt.kbhit():
#                    chr = msvcrt.getche()
#                    if ord(chr) == 13:
#                        break
#                    elif ord(chr) >= 32:
#                        self.input += chr
#                if len(self.input) == 0 and self.timedout:
#                    break
#
#    result = default
#    it = KeyboardThread()
#    it.start()
#    it.join(timeout)
#    it.timedout = True
#    if len(it.input) > 0:
#        # wait for rest of input
#        it.join()
#        result = it.input
#    return result