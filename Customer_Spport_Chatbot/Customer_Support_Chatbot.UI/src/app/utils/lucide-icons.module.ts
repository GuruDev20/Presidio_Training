import { NgModule } from '@angular/core';
import { Activity, Bell, ChevronLeft, ChevronRight, Clock, Clock10, LayoutDashboard, LogInIcon, LucideAngularModule, Menu, MessageCircle, MoreVertical, Plus, Settings, TerminalSquare, Ticket, X } from 'lucide-angular';
import { Camera, User, LogIn, LogOut, Sun, Moon } from 'lucide-angular';

@NgModule({
    imports: [
        LucideAngularModule.pick({
            Camera,
            User,
            LogIn,
            LogOut,
            Sun,
            Moon,
            Bell,
            Clock,
            LayoutDashboard,
            Settings,
            TerminalSquare,
            Ticket,
            Menu,
            Plus,
            MessageCircle,
            Activity,
            Clock10,
            ChevronLeft,
            ChevronRight,
            LogInIcon,
            MoreVertical,
            X
        })
    ],
    exports: [LucideAngularModule]
})
export class LucideIconsModule {}